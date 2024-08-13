using UnityEngine;
using System.Collections;
using System.Linq;
using Photon.Pun;
using Scripts.HitBox;
using Scripts.Character.Movement;
using Photon.Realtime;

namespace Scripts.Character
{
	public class PlayerController : BaseCharacterController
	{
		public InputHandler input = new InputHandler();
		[SerializeField] CharacterMovement movement = null;
		[SerializeField] PlayerUIHandler playerUIHandler = null;
		public override PhotonView Pv { get => _pv; protected set => _pv = value; }
		[SerializeField] PhotonView _pv = null;
		Transform thisTransform = null;
		Animator animator = null;
		public override PlayerStats PlayerStats => playerStats;
		PlayerStats playerStats = new PlayerStats();
		[SerializeField] Collider barrellRollColl = null;
		Collider coll = null;
		MeshRenderer mesh = null;

		[SerializeField] GameObject ExternalUI = null;

		[SerializeField] float laserFireRate = .5f;
		float laserFireTimer = 0f;
		[SerializeField] float laserAimThreshold = .95f;
		bool isLaserOverHeated = false;
		[SerializeField] float laserOverHeatedThreshold = 2f;
		float laserOverHeat = 0f;

		[SerializeField] float maxBombWaitTimer = 5;
		float bombTimer = 0;
		
		Vector3 targetPos;

		float missileFireTimer = 0;
		[SerializeField] float missileFireRate = 1;

		[SerializeField] float respawnTimer = 5f;
		[SerializeField] bool isDead = false;

		int actorNumber;

		[SerializeField] eAmmoAmount ammoAmount;
		Ammo ammo;

		//Animator fields
		readonly int BarrelRollDirectionAnimInt = Animator.StringToHash("BarrelRollDirection");
		readonly int isLoopAnimBool = Animator.StringToHash("isLoop");
		readonly int isUTurnAnimBool = Animator.StringToHash("isUTurn");

        #region Unity Calls

        void Awake()
		{
			animator = GetComponent<Animator>();
			mesh = GetComponent<MeshRenderer>();
			coll = GetComponent<Collider>();
			thisTransform = transform;
			input.ControlsEnabled = false;
			charData.health = charData.maxHealth;
			targetPos = thisTransform.forward;

			ammo = new Ammo(ammoAmount.Missiles, ammoAmount.Bombs);
		}

		private void Start()
		{
			var h = new ExitGames.Client.Photon.Hashtable() { { "Kills", 0 }, { "Deaths", 0 }, { "isInGame", true} };
			PhotonNetwork.LocalPlayer.SetCustomProperties(h);
			DeathMatchGameManager.instance.AddPlayerRef(this);
			actorNumber = _pv.Owner.ActorNumber;
			movement.OnExitManuever += EndManuever;
		}

		private void Update()
		{
			if (!_pv.IsMine || isDead)
				return;

			if(!DeathMatchGameManager.instance.matchEnded)
				DisplayPlayerList(input.DisplayPlayerList());

			LaserAim();

			MissileFireInput();
			BombFireInput();
			LaserFireInput();
			DoABarrelRoll();
			LoopInput();
			UTurnInput();			

			movement.OrbitMovement(input.HorizontalInput());
			movement.Boost(input.BoostInput());
			movement.Brake(input.BrakeInput());
		}

		private void OnDestroy()
		{
			DeathMatchGameManager.instance.RemovePlayerRef(this, actorNumber);
			movement.OnExitManuever -= EndManuever;
		}

		#endregion

		#region Fire Inputs
		private void LaserFireInput()
		{
			if (input.LaserInput() && laserFireTimer <= 0 && !isLaserOverHeated)
			{
				_pv.RPC("FireLaserRPC", RpcTarget.All, targetPos);
				laserFireTimer = laserFireRate;
				laserOverHeat += .6f;

				if (laserOverHeat >= laserOverHeatedThreshold)
				{
					isLaserOverHeated = true;
					StartCoroutine(LaserCoolDown());
				}
			}

			laserOverHeat -= Time.deltaTime;
			laserFireTimer -= Time.deltaTime;

			laserOverHeat = laserOverHeat < 0 ? 0 : laserOverHeat;
		}
		private void BombFireInput()
		{
			if(input.BombInput() && bombTimer <= 0)
			{
				if (ammo.CheckAmmo().Bombs > 0)
				{
					_pv.RPC("FireBombRPC", RpcTarget.All);
					bombTimer = maxBombWaitTimer;
					ammo.BombCount--;
				}
			}

			bombTimer -= Time.deltaTime;
			bombTimer = bombTimer < 0 ? 0 : bombTimer;
		}

		private void MissileFireInput()
		{
			if (input.MissileInput() && missileFireTimer <= 0)
			{
				if (ammo.CheckAmmo().Missiles > 0)
				{
					_pv.RPC("FireMissileRPC", RpcTarget.All);
					missileFireTimer = missileFireRate;
					ammo.MissileCount -= 2; //fires in pairs
				}
			}

			missileFireTimer -= Time.deltaTime;
			missileFireTimer = missileFireTimer < 0 ? 0 : missileFireTimer;
		}

		#endregion

        private static void DisplayPlayerList(bool isDisplay) =>
			DeathMatchGameManager.instance.DisplayPlayerList(isDisplay);

		IEnumerator LaserCoolDown()
		{
			yield return Wait.WaitForSeconds(2f);
			laserOverHeat = 0;
			isLaserOverHeated = false;
		}

        #region Manuever Inputs
        void LoopInput()
		{
			if (input.LoopInput() && movement.canExpendEnergy)
			{
				if ((movement.Manuever & eManuevers.bRollLoopUTurn) == 0)
				{
					animator.SetBool(isLoopAnimBool, true);
					input.FireEnabled = false;
					input.BoostEnabled = false;
					input.TurnEnabled = false;
				}
			}
		}

		void UTurnInput()
		{
			if(input.UTurnInput() && movement.canExpendEnergy)
			{
				if ((movement.Manuever & eManuevers.bRollLoopUTurn) == 0)
				{
					animator.SetBool(isUTurnAnimBool, true);
					input.FireEnabled = false;
					input.BoostEnabled = false;
					input.TurnEnabled = false;
				}
			}
		}

		void DoABarrelRoll()
		{
			if (input.BarrelRollInput() && movement.canExpendEnergy)
			{
				if ((movement.Manuever & eManuevers.bRollLoopUTurn) == 0)
				{
					int direction = input.HorizontalInput() > 0 ? 1 : -1;
					animator.SetInteger(BarrelRollDirectionAnimInt, direction);
					input.FireEnabled = false;
				}
			}
		}
        #endregion

        #region Animator Event Calls
        //used in animator key events to return back to idle
        void BackToIdle()
		{
			animator.SetInteger(BarrelRollDirectionAnimInt, 0);
		}

		void StartBarrellRoll()
		{
			barrellRollColl.enabled = true;
			coll.enabled = false;
		}

		void StopBarrellRoll()
		{
			barrellRollColl.enabled = false;
			animator.SetInteger(BarrelRollDirectionAnimInt, 0);
			coll.enabled = true;
		}

		#endregion

		public override bool ApplyHurtbox(sHitboxData hit)
		{
			if (hit.id == _pv.Owner.ActorNumber)
				return false;

			if (_pv.IsMine)
				_pv.RPC("ApplyDamageRPC", RpcTarget.All, hit.damage, hit.id);

			return true;
		}

		public void ApplyDamage(float damage, int id)
		{
			charData.health -= damage;
			playerUIHandler.UpdateHealthUI(charData);
			if (charData.health <= 0)
			{
				PlayerController projectileOwner = null;
				for(int i = 0; i < DeathMatchGameManager.instance.playerRefs.Count; i++)
				{
					if(DeathMatchGameManager.instance.playerRefs[i]._pv.Owner.ActorNumber == id)					
						projectileOwner = DeathMatchGameManager.instance.playerRefs[i];					
				}

				if (projectileOwner != null)				
					projectileOwner.AddPlayerKill();				

				Death();
			}
		}

		private void AddPlayerKill()
		{
			playerStats.AddKill();
			if (_pv.IsMine)
			{
				var h = new ExitGames.Client.Photon.Hashtable() { { "Kills", playerStats.KillCount } };
				PhotonNetwork.LocalPlayer.SetCustomProperties(h);
			}
			DeathMatchGameManager.instance.UpdatePlayerInfoInList();
		}

		[ContextMenu("Test Death")]
		public void Death()
		{			
			barrellRollColl.enabled = false;
			isDead = true;
			input.ControlsEnabled = false;
			ExternalUI.SetActive(false);
			coll.enabled = false;
			mesh.enabled = false;
			AddPlayerDeath();

			if (_pv.IsMine)
			{
				StartCoroutine(RespawnCoroutine(respawnTimer));
				playerUIHandler.ActivateRespawnTimer(true);
			}
		}

		void AddPlayerDeath()
		{
			PlayerStats.AddDeath();
			if (_pv.IsMine)
			{
				var h = new ExitGames.Client.Photon.Hashtable() { { "Deaths", playerStats.DeathCount } };
				PhotonNetwork.LocalPlayer.SetCustomProperties(h);
			}
			DeathMatchGameManager.instance.UpdatePlayerInfoInList();
		}

		public void Respawn()
		{
			input.ControlsEnabled = true;

			if (!_pv.IsMine)
				ExternalUI.SetActive(true);

			coll.enabled = true;
			mesh.enabled = true;
			isDead = false;
			charData.health = charData.maxHealth;
			playerUIHandler.UpdateHealthUI(charData);
			playerUIHandler.ActivateRespawnTimer(false);
		}

		public IEnumerator RespawnCoroutine(float seconds)
		{
			float timer = respawnTimer;

			playerUIHandler.RespawnTimerUIUpdate(timer);

			while (timer >= 0)
			{
				yield return Wait.WaitForSeconds(.2f);
				playerUIHandler.RespawnTimerUIUpdate(timer);
				timer -= .2f;
			}

			_pv.RPC("RespawnRPC", RpcTarget.All);
		}

		void LaserAim()
		{
			if(DeathMatchGameManager.instance.playerRefs.Count < 2)
			{
				targetPos = thisTransform.forward;
				return;
			}

			var playerTargets = DeathMatchGameManager.instance.playerRefs
				.Where(t => t != this && !t.isDead)
				.Select(t => t.transform.position)
				.ToArray();

			float shortestDistance = 300f;
			bool hasTarget = false;

			Debug.DrawRay(thisTransform.position, thisTransform.forward * 2, Color.green);

			for(int i = 0; i < playerTargets.Length; i++)
			{
				Vector3 targetVector;
				targetVector.x = playerTargets[i].x - thisTransform.position.x;
				targetVector.y = playerTargets[i].y - thisTransform.position.y;
				targetVector.z = playerTargets[i].z - thisTransform.position.z;
				
				Vector3 lockedTargetVector = targetVector - Vector3.Project(targetVector, transform.up);

				Debug.DrawRay(thisTransform.position, lockedTargetVector.normalized * 2, Color.magenta);

				if (Vector3.Dot(thisTransform.forward, lockedTargetVector.normalized) >= laserAimThreshold)
				{
					print("in angle");

					if (lockedTargetVector.sqrMagnitude < shortestDistance)
					{
						hasTarget = true;
						print("has target");
						targetPos = targetVector;
						shortestDistance = targetVector.sqrMagnitude;
					}
				}
			}

			if (!hasTarget)
			{
				targetPos = thisTransform.forward;
			}
		}

		void EndManuever()
		{
			input.FireEnabled = true;
			input.BoostEnabled = true;
			input.TurnEnabled = true;
		}

		#region Test Damage
		[ContextMenu("Test Damage")]
		void ApplyDamage()
		{
			charData.health -= 30;
			playerUIHandler.UpdateHealthUI(charData);
			if (charData.health <= 0)
			{
				Death();
			}
		}
		#endregion

		void OnGUI()
		{
			GUI.Label(new Rect(0, 0, 100, 100), "FPS: " + (int)(1.0f / Time.smoothDeltaTime));
		}
	}	
}