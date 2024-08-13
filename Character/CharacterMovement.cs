using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Scripts.Character.Movement
{
	[System.Serializable]
	public struct sMovementValues
	{
		public float thrustSpeed;
		public float maxThrustSpeed;
		public float initialThrustSpeed;
		public float minimumThrustSpeed;
	}

	[System.Serializable]
	public struct sEnergyValues
	{
		public float energy;
		public float maxEnergy;
		public float regenerateEnergyRate;
		public float energyCoolDownWaitSeconds;
	}

	[Flags]
	public enum eManuevers
	{
		none = 0,
		idle = 1 << 0,
		boost = 1 << 1,
		bRoll = 1 << 2,
		Loop = 1 << 3,
		braking = 1 << 4,
		UTurn = 1 << 5,
		LoopUTurn = Loop | UTurn,
		bRollAndLoop = bRoll | Loop,
		bRollBoost = bRoll | boost,
		boostLoop = boost | Loop,
		bRollLoopUTurn = Loop | UTurn | bRoll,
		bRollBoostLoop = bRoll | boost | Loop,
		brakeBoostLoop = boost | Loop | braking,
		brakeBoostLoopUturn = braking | Loop | boost | UTurn,
		allManuever = bRoll | Loop | boost | braking | UTurn,
	}

	public class CharacterMovement : MonoBehaviour
	{		
		[SerializeField] Vector3 pivot = Vector3.zero;
		Transform thisTransform = null;
		[SerializeField] Animator anim = null;
		PlayerUIHandler playerUIHandler = null;

		public float PivotDistance = 5f;
		[SerializeField] float maxTurn = 30f;
		[SerializeField] float turnMod = 400;

		float Horz = 0;

		Action<eManuevers> onEnterManuever = delegate { };
		public event Action<eManuevers> OnEnterManuever
		{
			add
			{
				if(!onEnterManuever.GetInvocationList().Contains(value))
				{
					onEnterManuever += value;
				}
			}

			remove
			{
				if (onEnterManuever.GetInvocationList().Contains(value))
				{
					onEnterManuever -= value;
				}
			}
		}

		Action onExitManuever = delegate { };
		public event Action OnExitManuever
		{
			add
			{
				if (!onExitManuever.GetInvocationList().Contains(value))
				{
					onExitManuever += value;
				}
			}

			remove
			{
				if (onExitManuever.GetInvocationList().Contains(value))
				{
					onExitManuever -= value;
				}
			}
		}

		[SerializeField] sMovementValues _movementValues = new sMovementValues();
		[SerializeField] sEnergyValues _energyValues = new sEnergyValues();
		
		eManuevers manuever = eManuevers.none;
		public eManuevers Manuever => manuever;

		[SerializeField] float baseEnergyConsumption = 15f;
		public bool canExpendEnergy { get; private set; } = true;
		[SerializeField] float boostingMod = 1.1f;
		[SerializeField] float brakingMod = .9f;

        #region Unity Calls

        void Awake()
		{
			thisTransform = GetComponent<Transform>();
			_movementValues.thrustSpeed = _movementValues.initialThrustSpeed;
			_energyValues.energy = _energyValues.maxEnergy;
			playerUIHandler = GetComponent<PlayerUIHandler>();
		}

		private void Start()
		{
			pivot = GameObject.FindGameObjectWithTag("MainWorld").transform.position;
		}

		private void Update()
		{
			RegenerateEnergy();
		}

        #endregion

        public void Boost(bool boostInput)
		{
			if (_energyValues.energy <= 0)
			{
				canExpendEnergy = false;
				StartCoroutine(BoostCoolDown());
			}

			if (boostInput && canExpendEnergy && (manuever & eManuevers.Loop) != eManuevers.Loop)
			{
				manuever |= eManuevers.boost;
				ExpendEnergy();
				ApplyBoost();
			}
			else
			{
				if ((manuever & eManuevers.boost) == eManuevers.boost)
					manuever ^= eManuevers.boost;
			}

			playerUIHandler.UpdateBoostEnergyUI(_energyValues.energy, _energyValues.maxEnergy);
		}

		public void ExpendEnergy(float consumption = 0)
		{
			if (consumption == 0)
				consumption = baseEnergyConsumption;

			_energyValues.energy -= consumption * Time.deltaTime;
			_energyValues.energy = Mathf.Clamp(_energyValues.energy, 0, _energyValues.maxEnergy);			
		}

		private void RegenerateEnergy()
		{
			if((manuever & eManuevers.bRollBoostLoop) == 0)
			{
				_energyValues.energy += _energyValues.regenerateEnergyRate * Time.deltaTime;
				_energyValues.energy = Mathf.Clamp(_energyValues.energy, 0, _energyValues.maxEnergy);
			}
		}

		public void OrbitMovement(float horz)
		{
			SetToInitialThrust();

			if(horz == 0 && Horz != 0)
			{
				if(Horz > 0)
				{
					Horz -= 300 * Time.deltaTime;
					Horz = Mathf.Clamp(Horz, 0, maxTurn);
				}
				else
				{
					Horz += 300 * Time.deltaTime;
					Horz = Mathf.Clamp(Horz, -maxTurn, 0);
				}						
			}
			else
			{
				Horz += horz * turnMod * Time.deltaTime;
				Horz = Mathf.Clamp(Horz, -maxTurn, maxTurn);
			}

			anim.SetFloat("Turn", Horz);

			thisTransform.Rotate(0, 0, -Horz * Time.deltaTime);
			thisTransform.RotateAround(pivot, transform.right, _movementValues.thrustSpeed * Time.deltaTime);
		}

		public void Brake(bool isBraking)
		{
			if( !isBraking || ((eManuevers.boostLoop) & manuever) != 0)
			{
				if ((manuever & eManuevers.braking) == eManuevers.braking)
					manuever ^= eManuevers.braking;
				return;
			}

			manuever |= eManuevers.braking;
			_movementValues.thrustSpeed *= brakingMod;
			_movementValues.thrustSpeed = Mathf.Clamp(_movementValues.thrustSpeed, _movementValues.minimumThrustSpeed, _movementValues.thrustSpeed);
		}

		void ApplyBoost()
		{
			_movementValues.thrustSpeed *= boostingMod;
			_movementValues.thrustSpeed = Mathf.Clamp(_movementValues.thrustSpeed, _movementValues.initialThrustSpeed, _movementValues.maxThrustSpeed);
		}

		void SetToInitialThrust()
		{
			if ((manuever & eManuevers.brakeBoostLoopUturn) != 0)
				return;

			if (_movementValues.thrustSpeed > _movementValues.initialThrustSpeed)
			{
				_movementValues.thrustSpeed -= 30 * Time.deltaTime;
				_movementValues.thrustSpeed = Mathf.Clamp(_movementValues.thrustSpeed, _movementValues.initialThrustSpeed, _movementValues.maxThrustSpeed);
			}
			else
			{
				_movementValues.thrustSpeed += 5 * Time.deltaTime;
				_movementValues.thrustSpeed = Mathf.Clamp(_movementValues.thrustSpeed, _movementValues.minimumThrustSpeed, _movementValues.initialThrustSpeed);
			}
		}

		public void BeginManuever(eManuevers m)
		{
			switch(m)
			{
				case eManuevers.Loop:
					manuever = eManuevers.Loop;
					_movementValues.thrustSpeed = 5f;
					break;
				case eManuevers.bRoll:
					manuever |= eManuevers.bRoll;
					break;
				case eManuevers.UTurn:
					manuever |= eManuevers.UTurn;
					_movementValues.thrustSpeed = 0;
					break;
				default:
					break;
			}
			onEnterManuever(m);			
		}

		public void EndManuever(eManuevers m)
		{
			switch (m)
			{
				case eManuevers.Loop:
					manuever ^= eManuevers.Loop;
					_movementValues.thrustSpeed = _movementValues.initialThrustSpeed;
					break;
				case eManuevers.bRoll:
					manuever ^= eManuevers.bRoll;
					break;
				case eManuevers.UTurn:
					manuever ^= eManuevers.UTurn;
					_movementValues.thrustSpeed = _movementValues.initialThrustSpeed;
					transform.Rotate(0, 0, 180);
					break;
				default:
					break;
			}
			onExitManuever();
		}

		IEnumerator BoostCoolDown()
		{
			yield return Wait.WaitForSeconds(_energyValues.energyCoolDownWaitSeconds);
			canExpendEnergy = true;
		}
	}
}