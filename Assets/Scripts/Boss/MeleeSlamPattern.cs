using UnityEngine;
using CMGTSA.Battle;

namespace CMGTSA.Boss
{
    [CreateAssetMenu(fileName = "MeleeSlam", menuName = "CMGTSA/Boss/Patterns/Melee Slam")]
    public class MeleeSlamPattern : SOBossAttackPattern
    {
        [Tooltip("Seconds the boss winds up before the swing lands.")]
        public float windupSeconds = 0.5f;

        public DamageData damage;

        public override string Describe()
        {
            return $"Melee Slam ({damage?.damage ?? 0} dmg, {windupSeconds:0.##}s windup)";
        }

        public override IBossPatternRuntime CreateRuntime()
        {
            return new MeleeSlamRuntime(this);
        }
    }
}
