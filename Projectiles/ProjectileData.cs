using UnityEngine;

namespace Scripts.Projectiles
{
    [CreateAssetMenu(fileName = "New Projectile Data", menuName = "Projectile Data/Generic Projectile")]
    public class ProjectileData : ScriptableObject
    {
        public float damage;
        public float speed;
    }
}