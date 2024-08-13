using Scripts.HitBox;
using UnityEngine;

namespace Scripts.Projectiles
{
    public class BombProperties : GenericProjectileProperties
    {
        protected override ProjectileData Data => data;
        [SerializeField] BombData data = null;

        MissileMovement movement = null;
        HitBoxData _hb = null;
        GameObject explosion = null;

        #region Unity Functions

        private void Awake()
        {
            _hb = GetComponent<HitBoxData>();
            movement = GetComponent<MissileMovement>();

            movement.ThrustSpeed = data.speed;
            _hb.hbData.damage = data.damage;
            explosion = data.explosion;
        }

        private void OnEnable()
        {
            _hb.AddHitboxEvent(OnHitboxEvent);
        }

        private void OnDisable()
        {
            _hb.RemoveHitboxEvent(OnHitboxEvent);
        }

        private void Update()
        {
            DestroyTime();
        }

        #endregion

        protected override void DestroyTime()
        {
            if (timer <= 0)
            {
                GameObject exp = Instantiate(explosion, transform.position, transform.rotation);
                exp.GetComponent<HitBoxData>().hbData.id = _hb.hbData.id;
                
                gameObject.SetActive(false);
                return;
            }

            timer -= Time.deltaTime;
        }

        private void OnHitboxEvent(int data)
        {
            GameObject exp = Instantiate(this.data.explosion, transform.position, transform.rotation);
            exp.GetComponent<HitBoxData>().hbData.id = _hb.hbData.id;
        }        

        public override GameObject GetGameObject() => gameObject;

        public override void Initialize(int id)
        {
            _hb.hbData.id = id;
            gameObject.SetActive(true);
            timer = destroyTimer;
        }
    }
}