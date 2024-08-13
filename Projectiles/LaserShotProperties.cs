using Scripts.HitBox;
using Scripts.Projectiles;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class LaserShotProperties : GenericProjectileProperties, IHurtBox
{
    LaserShotMovement movement = null;
    HitBoxData _hb = null;
    protected override ProjectileData Data => data;
    [SerializeField] ProjectileData data = null;

    public bool ApplyHurtbox(sHitboxData hit)
    {
        movement.RotateOnReflect();
        return true;
    }

    private void Awake()
    {
        _hb = GetComponent<HitBoxData>();
        movement = GetComponent<LaserShotMovement>();

        _hb.hbData.damage = data.damage;
        movement.Speed = data.speed;
    }

    private void Update()
    {
        DestroyTime();
    }

    public override GameObject GetGameObject() => gameObject;

    public override void Initialize(int id)
    {
        _hb.hbData.id = id;
        timer = destroyTimer;
        gameObject.SetActive(true);
        movement.Initialize();
    }

    protected override void DestroyTime()
    {
        if (timer <= 0)
        {          
            gameObject.SetActive(false);
            return;
        }

        timer -= Time.deltaTime;
    }
}
