using UnityEngine;

namespace CMGTSA.Boss
{
    public class ProjectileSprayRuntime : IBossPatternRuntime
    {
        private readonly ProjectileSprayPattern pattern;
        private int fired;
        private float nextFireTime;
        private float elapsed;

        public ProjectileSprayRuntime(ProjectileSprayPattern pattern)
        {
            this.pattern = pattern;
        }

        public bool IsFinished => fired >= pattern.projectileCount;

        public void OnBegin(IBossContext ctx)
        {
            fired = 0;
            elapsed = 0f;
            nextFireTime = 0f;
        }

        public void Tick(float dt, IBossContext ctx)
        {
            if (IsFinished) return;
            elapsed += dt;

            while (!IsFinished && elapsed >= nextFireTime)
            {
                FireOne(ctx, fired);
                fired++;
                nextFireTime += pattern.intervalSeconds;
            }
        }

        private void FireOne(IBossContext ctx, int index)
        {
            Vector3 toPlayer = ctx.PlayerPosition - ctx.BossPosition;
            if (toPlayer.sqrMagnitude < 0.0001f)
            {
                toPlayer = Vector3.right;
            }
            Vector2 baseDir = ((Vector2)toPlayer).normalized;

            float t = pattern.projectileCount <= 1 ? 0.5f : (float)index / (pattern.projectileCount - 1);
            float angleOffset = Mathf.Lerp(-pattern.spreadDegrees * 0.5f, pattern.spreadDegrees * 0.5f, t);
            Vector2 dir = Rotate(baseDir, angleOffset);

            ctx.SpawnProjectile(pattern.projectilePrefab, ctx.BossPosition, dir, pattern.projectileSpeed, pattern.damage);
        }

        private static Vector2 Rotate(Vector2 v, float degrees)
        {
            float rad = degrees * Mathf.Deg2Rad;
            float c = Mathf.Cos(rad);
            float s = Mathf.Sin(rad);
            return new Vector2(v.x * c - v.y * s, v.x * s + v.y * c);
        }
    }
}
