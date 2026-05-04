using UnityEngine;

namespace CMGTSA.Boss
{
    [CreateAssetMenu(fileName = "BossPhase", menuName = "CMGTSA/Boss/Phase")]
    public class BossPhase : ScriptableObject
    {
        [Tooltip("Editor-only label; appears in HP bar tooltips and logs.")]
        public string displayName;

        [Range(0f, 1f)]
        [Tooltip("Phase becomes active when boss HP fraction drops to or below this value. Phase 1 should use 1.0.")]
        public float hpThresholdEnter = 1f;

        [Tooltip("Patterns the boss can cast in this phase. Pattern selection is round-robin in slice 6.")]
        public SOBossAttackPattern[] patterns;

        [Min(0.1f)]
        [Tooltip("Seconds the boss spends in chase between casts in this phase.")]
        public float castIntervalSeconds = 2f;
    }
}
