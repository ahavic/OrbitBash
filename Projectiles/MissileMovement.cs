using Scripts.HitBox;
using Scripts.ObjectPools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Projectiles
{
	public class MissileMovement : MonoBehaviour
	{
		Transform pivot = null;

		public float ThrustSpeed { private get => ThrustSpeed; set => thrustSpeed = value; }
		float thrustSpeed;

		private void Start()
		{
			pivot = DeathMatchGameManager.planet.transform;
		}

		private void Update()
		{
			transform.RotateAround(pivot.position, transform.right, thrustSpeed * Time.deltaTime);
		}		
	}
}