namespace CMGTSA.Boss
{
    public class BossPhaseSelector
    {
        private readonly BossPhase[] phases;

        public BossPhaseSelector(BossPhase[] phases)
        {
            this.phases = phases;
        }

        public int PhaseCount => phases == null ? 0 : phases.Length;

        public int SelectPhase(float hpFraction)
        {
            if (phases == null || phases.Length == 0) return 0;
            int last = 0;
            for (int i = 0; i < phases.Length; i++)
            {
                if (phases[i] == null) continue;
                if (hpFraction <= phases[i].hpThresholdEnter)
                {
                    last = i;
                }
                else
                {
                    break;
                }
            }
            return last;
        }

        public bool HasChangedSince(int previousIndex, float hpFraction)
        {
            return SelectPhase(hpFraction) != previousIndex;
        }

        public BossPhase GetPhase(int index)
        {
            if (phases == null || index < 0 || index >= phases.Length) return null;
            return phases[index];
        }
    }
}
