using Photon.Realtime;
using Scripts.ObjectPools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Projectiles
{
    public class LaserFire : MonoBehaviour
    {
        [SerializeField] GameObject laserPrefab = null;
        Type poolType;

        private void Awake()
        {
            poolType = laserPrefab.GetComponent<IPoolObject>().GetType();
        }

        public void Fire(int id, Vector3 target)
        {
            transform.rotation = Quaternion.LookRotation(target);

            IPoolObject laserPoolObj = MissilePool.Instance.GetFromPool(poolType, transform.position, transform.rotation);
            laserPoolObj.Initialize(id);
        }
    }
}