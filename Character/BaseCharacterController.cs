using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Scripts.HitBox;

namespace Scripts.Character
{
	public abstract class BaseCharacterController : MonoBehaviourPun, IHurtBox
	{
		public CharacterData charData;
		public abstract PhotonView Pv { get; protected set; }
		[SerializeField] public abstract PlayerStats PlayerStats { get; }

		public abstract bool ApplyHurtbox(sHitboxData hit);
	}
}