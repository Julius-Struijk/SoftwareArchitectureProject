using System;
using UnityEngine;
using CMGTSA.FSM;

namespace CMGTSA.Boss
{
    public class BossFSM : FSM
    {
        public readonly BossIdleState idleState;
        public readonly BossChaseState chaseState;
        public readonly BossCastPatternState castState;
        public readonly BossDeadState deadState;

        private readonly Func<bool> isDead;

        public BossFSM(BossIdleState idle, BossChaseState chase, BossCastPatternState cast,
                       BossDeadState dead, Func<bool> isDead)
        {
            idleState  = idle;
            chaseState = chase;
            castState  = cast;
            deadState  = dead;
            this.isDead = isDead;

            idleState.transitions.Add(new Transition(idleState.ShouldActivate, chaseState));
            chaseState.transitions.Add(new Transition(chaseState.ReadyToCast, castState));
            castState.transitions.Add(new Transition(castState.IsFinished, chaseState));

            currentState = idleState;
        }

        public override void Enter()
        {
            base.Enter();
            Debug.Log("Boss FSM is active.");
            currentState.Enter();
        }

        public override void Step()
        {
            if (isDead != null && isDead() && currentState != deadState)
            {
                currentState.Exit();
                currentState = deadState;
                currentState.Enter();
                return;
            }
            base.Step();
        }
    }
}
