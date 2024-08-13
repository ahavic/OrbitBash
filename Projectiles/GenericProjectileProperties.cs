using Scripts.Projectiles;
using System;
using UnityEngine;

public abstract class GenericProjectileProperties : MonoBehaviour, IPoolObject
{
    public Type PoolType => GetType();
    protected abstract ProjectileData Data { get; }
    [SerializeField] protected float destroyTimer = 2.5f;
    
    protected float timer = 0;

    protected abstract void DestroyTime();
    public abstract GameObject GetGameObject();
    public abstract void Initialize(int id);
}
