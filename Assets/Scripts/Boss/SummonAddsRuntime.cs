using UnityEngine;

namespace CMGTSA.Boss
{
    public class SummonAddsRuntime : IBossPatternRuntime
    {
        private readonly SummonAddsPattern pattern;
        private bool finished;

        public SummonAddsRuntime(SummonAddsPattern pattern)
        {
            this.pattern = pattern;
        }

        public bool IsFinished => finished;

        public void OnBegin(IBossContext ctx)
        {
            finished = false;
        }

        public void Tick(float dt, IBossContext ctx)
        {
            if (finished) return;

            int count = Mathf.Max(1, pattern.addCount);
            int attempts = Mathf.Max(1, pattern.maxAttempts);

            for (int i = 0; i < count; i++)
            {
                for (int attempt = 0; attempt < attempts; attempt++)
                {
                    float jitter = attempt == 0 ? 0f : Random.Range(-0.5f, 0.5f);
                    float angle = ((i + jitter) / count) * Mathf.PI * 2f;
                    Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * pattern.spawnRadius;
                    Vector3 candidate = ctx.BossPosition + offset;

                    if (ctx.TrySampleNavMesh(candidate, pattern.sampleDistance, out Vector3 valid))
                    {
                        ctx.SpawnAdd(pattern.addPrefab, valid);
                        break;
                    }
                }
            }

            finished = true;
        }
    }
}
