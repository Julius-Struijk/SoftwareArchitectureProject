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
            for (int i = 0; i < count; i++)
            {
                float angle = (i / (float)count) * Mathf.PI * 2f;
                Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * pattern.spawnRadius;
                ctx.SpawnAdd(pattern.addPrefab, ctx.BossPosition + offset);
            }
            finished = true;
        }
    }
}
