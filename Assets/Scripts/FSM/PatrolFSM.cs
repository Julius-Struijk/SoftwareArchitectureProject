using System;
using UnityEngine;
using UnityEngine.AI;
using CMGTSA.Enemies;

namespace CMGTSA.FSM
{
    /// <summary>
    /// Patrol sub-FSM: cycle MoveTo → Idle → MoveTo around a random point near the start position.
    /// </summary>
    public class PatrolFSM : FSM
    {
        private MoveToState moveToState;
        private IdleState idleState;

        public Action onStartMoving;
        public Action onStartIdling;

        public PatrolFSM(NavMeshAgent navMeshAgent, Enemy pEnemy)
        {
            enemy = pEnemy;

            moveToState = new MoveToState(navMeshAgent, enemy);
            idleState = new IdleState(enemy);

            idleState.transitions.Add(new Transition(idleState.IdleTimeReached, moveToState));
            moveToState.transitions.Add(new Transition(moveToState.TargetReached, idleState));

            moveToState.onEnter += SetNextSeekPosition;
            moveToState.onEnter += StartMoving;
            idleState.onEnter += StartIdling;
        }

        public override void Enter()
        {
            base.Enter();
            Debug.Log("Entered Patrol state.");
            currentState = moveToState;
            currentState.Enter();
        }

        private void StartMoving() => onStartMoving?.Invoke();
        private void StartIdling() => onStartIdling?.Invoke();

        private void SetNextSeekPosition()
        {
            enemy.TargetPosition = enemy.GetRandomPointAroundCurrentWaypoint();
        }
    }
}
