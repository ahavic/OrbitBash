using Scripts.ObjectPools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Projectiles
{
    [System.Serializable]
    public class ProjectileFire : MonoBehaviour, IFire
    {
        [SerializeField] GameObject projectilePrefab = null;
        Type poolType;

        private void Awake()
        {
            poolType = projectilePrefab.GetComponent<IPoolObject>().GetType();
        }

        public void Fire(int id)
        {
            IPoolObject missilePoolObj = MissilePool.Instance.GetFromPool(poolType, transform.position, transform.rotation);
            missilePoolObj.Initialize(id);
        }
    }
}