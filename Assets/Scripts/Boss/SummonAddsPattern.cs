using UnityEngine;

namespace CMGTSA.Boss
{
    [CreateAssetMenu(fileName = "SummonAdds", menuName = "CMGTSA/Boss/Patterns/Summon Adds")]
    public class SummonAddsPattern : SOBossAttackPattern
    {
        public GameObject addPrefab;
        [Min(1)] public int addCount = 4;
        public float spawnRadius = 2f;

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
