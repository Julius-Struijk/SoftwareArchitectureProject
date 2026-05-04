using UnityEngine;
using CMGTSA.Battle;

namespace CMGTSA.Boss
{
    [CreateAssetMenu(fileName = "ProjectileSpray", menuName = "CMGTSA/Boss/Patterns/Projectile Spray")]
    public class ProjectileSprayPattern : SOBossAttackPattern
    {
        public GameObject projectilePrefab;
        [Min(1)] public int projectileCount = 3;
        public float spreadDegrees = 30f;
        public float projectileSpeed = 5f;
        public float intervalSeconds = 0.2f;
        public DamageData damage;

        public override string Describe()
        {
            return $"Projectile Spray ({projectileCount} shots, {spreadDegrees}° spread)";
        }

        public override IBossPatternRuntime CreateRuntime()
        {
            return new ProjectileSprayRuntime(this);
        }
    }
}
