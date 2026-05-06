using System;
using CMGTSA.FSM;

namespace CMGTSA.Boss
{
    public class BossIdleState : State
    {
        private readonly Func<bool> shouldActivate;

        public BossIdleState(Func<bool> shouldActivate)
        {
            this.shouldActivate = shouldActivate;
        }

        public bool ShouldActivate()
        {
            return shouldActivate != null && shouldActivate();
        }
    }
}
