using UnityEngine;

namespace Scripts.Projectiles
{
    [CreateAssetMenu(fileName = "New Projectile Data", menuName = "Projectile Data/Bomb Data")]
    public class BombData : ProjectileData
    {
        public GameObject explosion;
    }
}