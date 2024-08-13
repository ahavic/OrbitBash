using Scripts.ObjectPools;
using System;
using UnityEngine;

namespace Scripts.Projectiles
{
    public class BombFire : MonoBehaviour, IFire
    {
        [SerializeField] GameObject missilePrefab = null;
        Type poolType;

        private void Awake()
        {
            poolType = missilePrefab.GetComponent<IPoolObject>().GetType();
        }

        public void Fire(int id)
        {
            IPoolObject missilePoolObj = MissilePool.Instance.GetFromPool(poolType, transform.position, transform.rotation);
            //NOTE** remove tupple and let IPoolObject set active true in initialize method?
            missilePoolObj.Initialize(id);
        }
    }
}