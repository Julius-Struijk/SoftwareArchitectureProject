using UnityEngine;
using CMGTSA.Core;

namespace CMGTSA.Boss
{
    public class BossHealthModel
    {
        public int Max { get; }
        public int Current { get; private set; }
        public int PhaseIndex { get; private set; }

        public float Fraction => Max <= 0 ? 0f : (float)Current / Max;

        public BossHealthModel(int max, int phaseIndex)
        {
            Max = Mathf.Max(1, max);
            Current = Max;
            PhaseIndex = phaseIndex;
        }

        public void SetPhaseIndex(int index)
        {
            PhaseIndex = index;
        }

        public bool Damage(int amount)
        {
            if (amount <= 0) return Current == 0;
            Current = Mathf.Max(0, Current - amount);
            EventBus<BossHPChangedEvent>.Publish(new BossHPChangedEvent(Current, Max, PhaseIndex));
            return Current == 0;
        }
    }
}
