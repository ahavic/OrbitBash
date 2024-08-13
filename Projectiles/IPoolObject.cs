using Scripts.ObjectPools;
using System;
using UnityEngine;

namespace Scripts.Projectiles
{
    public interface IPoolObject
    {
        Type PoolType { get; }
        void Initialize(int id);
        GameObject GetGameObject();
    }
}