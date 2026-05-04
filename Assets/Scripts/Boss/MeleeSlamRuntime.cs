namespace CMGTSA.Boss
{
    public class MeleeSlamRuntime : IBossPatternRuntime
    {
        private readonly MeleeSlamPattern pattern;
        private float elapsed;
        private bool fired;

        public MeleeSlamRuntime(MeleeSlamPattern pattern)
        {
            this.pattern = pattern;
        }

        public bool IsFinished => fired;

        public void OnBegin(IBossContext ctx)
        {
            elapsed = 0f;
            fired = false;
        }

        public void Tick(float dt, IBossContext ctx)
        {
            if (fired) return;
            elapsed += dt;
            if (elapsed >= pattern.windupSeconds)
            {
                ctx.RequestMeleeAttack(pattern.range, pattern.damage);
                fired = true;
            }
        }
    }
}
