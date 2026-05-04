using CMGTSA.Core;

namespace CMGTSA.Boss
{
    public readonly struct BossHPChangedEvent : IGameEvent
    {
        public readonly int Current;
        public readonly int Max;
        public readonly int PhaseIndex;

        public BossHPChangedEvent(int current, int max, int phaseIndex)
        {
            Current = current;
            Max = max;
            PhaseIndex = phaseIndex;
        }
    }
}
