using UnityEngine;

namespace CMGTSA.Boss
{
    [CreateAssetMenu(fileName = "SummonAdds", menuName = "CMGTSA/Boss/Patterns/Summon Adds")]
    public class SummonAddsPattern : SOBossAttackPattern
    {
        public GameObject addPrefab;
        [Min(1)] public int addCount = 4;
        public float spawnRadius = 2f;

        [Tooltip("How many positions to sample per add before giving up. Higher = denser placement near walls; lower = faster.")]
        [Min(1)] public int maxAttempts = 4;

        [Tooltip("NavMesh.SamplePosition radius. Larger = adds may snap further from the requested ring; too small = many skips.")]
        [Min(0.1f)] public float sampleDistance = 1f;

        public override string Describe()
        {
            return $"Summon {addCount} Adds (r={spawnRadius})";
        }

        public override IBossPatternRuntime CreateRuntime()
        {
            return new SummonAddsRuntime(this);
        }
    }
}
