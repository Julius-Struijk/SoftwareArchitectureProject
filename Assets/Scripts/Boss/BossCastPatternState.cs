using System;
using UnityEngine;
using CMGTSA.FSM;

namespace CMGTSA.Boss
{
    public class BossCastPatternState : State
    {
        private readonly Func<IBossPatternRuntime> nextRuntimeProvider;
        private readonly Func<IBossContext> contextProvider;
        private IBossPatternRuntime current;

        public BossCastPatternState(Func<IBossPatternRuntime> nextRuntime,
                                    Func<IBossContext> contextProvider)
        {
            nextRuntimeProvider = nextRuntime;
            this.contextProvider = contextProvider;
        }

        public override void Enter()
        {
            base.Enter();
            current = nextRuntimeProvider != null ? nextRuntimeProvider() : null;
            current?.OnBegin(contextProvider != null ? contextProvider() : null);
        }

        public override void Step()
        {
            base.Step();
            if (current == null) return;
            current.Tick(Time.deltaTime, contextProvider != null ? contextProvider() : null);
        }

        public bool IsFinished()
        {
            return current == null || current.IsFinished;
        }
    }
}
