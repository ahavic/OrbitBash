using Scripts.HitBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Projectiles
{
	public class LaserShotMovement : MonoBehaviour
	{
		Rigidbody rb = null;

		bool isReflected = false;

		public float Speed { private get => speed; set => speed = value; }
		float speed;
		
		private void Awake()
		{
			rb = GetComponent<Rigidbody>();
		}

		public void Initialize()
		{			
			isReflected = false;
		}

		private void FixedUpdate()
		{
			rb.MovePosition(transform.position + transform.forward * Time.deltaTime * speed);
		}	

		public void RotateOnReflect()
		{
			if (isReflected)
				return;

			print("reflected");
			transform.Rotate(Random.Range(155,205), Random.Range(-30,30), 0);
			isReflected = true;
		}
	}
}
