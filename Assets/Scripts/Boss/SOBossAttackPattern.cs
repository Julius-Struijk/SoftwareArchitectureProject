using UnityEngine;

namespace CMGTSA.Boss
{
    public abstract class SOBossAttackPattern : ScriptableObject
    {
        [Tooltip("Seconds the boss waits in chase before this pattern can be picked again.")]
        public float cooldownSeconds = 2f;

        [Tooltip("Approximate engagement distance the boss prefers before casting.")]
        public float range = 4f;

        public abstract string Describe();
        public abstract IBossPatternRuntime CreateRuntime();
    }
}
