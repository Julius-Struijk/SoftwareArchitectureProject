using System;
using UnityEngine;
using CMGTSA.Core;
using CMGTSA.Enemies;
using CMGTSA.FSM;

namespace CMGTSA.Boss
{
    public class BossDeadState : State
    {
        private readonly Func<BossData> dataProvider;
        private readonly Transform bossTransform;
        private readonly Action onEntered;

        public BossDeadState(Func<BossData> dataProvider, Transform bossTransform, Action onEntered)
        {
            this.dataProvider = dataProvider;
            this.bossTransform = bossTransform;
            this.onEntered = onEntered;
        }

        public override void Enter()
        {
            base.Enter();
            BossData data = dataProvider != null ? dataProvider() : null;
            EventBus<BossEncounterEndedEvent>.Publish(new BossEncounterEndedEvent(data, true));
            if (data != null && bossTransform != null)
            {
                EventBus<EnemyDiedEvent>.Publish(new EnemyDiedEvent(
                    data.xpReward, data.moneyReward, bossTransform.position, null));
            }
            onEntered?.Invoke();
        }
    }
}
