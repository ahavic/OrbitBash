using Scripts.HitBox;
using UnityEngine;


namespace Scripts.Projectiles
{
    public class MissileProperties : GenericProjectileProperties
    {
        
        MissileMovement movement;
        HitBoxData _hb = null;
        protected override ProjectileData Data => data;
        [SerializeField] ProjectileData data = null;

        private void Awake()
        {
            _hb = GetComponent<HitBoxData>();
            movement = GetComponent<MissileMovement>();

            _hb.hbData.damage = data.damage;
            movement.ThrustSpeed = data.speed;
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
}