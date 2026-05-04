using System;
using UnityEngine;
using UnityEngine.AI;
using CMGTSA.FSM;

namespace CMGTSA.Boss
{
    public class BossChaseState : State
    {
        private readonly Transform bossTransform;
        private readonly Transform playerTransform;
        private readonly NavMeshAgent agent;
        private readonly Func<float> castIntervalProvider;
        private readonly Func<float> engagementRangeProvider;

        private float chaseStartTime;

        public BossChaseState(Transform boss, Transform player, NavMeshAgent agent,
                              Func<float> castInterval, Func<float> engagementRange)
        {
            bossTransform = boss;
            playerTransform = player;
            this.agent = agent;
            castIntervalProvider = castInterval;
            engagementRangeProvider = engagementRange;
        }

        public override void Enter()
        {
            base.Enter();
            chaseStartTime = Time.time;
        }

        public override void Step()
        {
            base.Step();
            if (playerTransform == null || bossTransform == null) return;
            if (agent == null || !agent.enabled || !agent.isOnNavMesh) return;
            agent.SetDestination(playerTransform.position);
        }

        public bool ReadyToCast()
        {
            if (playerTransform == null || bossTransform == null) return false;
            float distance = Vector3.Distance(bossTransform.position, playerTransform.position);
            bool inRange = distance <= engagementRangeProvider();
            bool intervalElapsed = Time.time >= chaseStartTime + castIntervalProvider();
            return inRange && intervalElapsed;
        }
    }
}
