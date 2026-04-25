using System;
using UnityEngine;
using UnityEngine.AI;
using CMGTSA.Enemies;

namespace CMGTSA.FSM
{
    /// <summary>
    /// Alert sub-FSM: chase, attack, or seek-around-the-last-known-position when the target is lost.
    /// </summary>
    public class SeekAndAttackFSM : FSM
    {
        private MoveToState chaseState;
        private AttackState attackState;
        private MoveToState seekState;
        private IdleState seekIdleState;

        public Action onStartMoving;
        public Action onStartIdling;
        public Action onStartAttacking;

        private Transform transform;
        private Transform target;

        public SeekAndAttackFSM(NavMeshAgent navMeshAgent, Enemy pEnemy)
        {
            transform = pEnemy.stateOwnerTransform;
            target = pEnemy.target;
            enemy = pEnemy;

            chaseState = new MoveToState(navMeshAgent, enemy, true);
            attackState = new AttackState(transform, target, enemy);
            seekState = new MoveToState(navMeshAgent, enemy);
            seekIdleState = new IdleState(enemy);

            attackState.transitions.Add(new Transition(attackState.FinishedAttacking, chaseState));

            chaseState.transitions.Add(new Transition(AttackTargetFound, attackState));
            chaseState.transitions.Add(new Transition(ChaseTargetLost, seekIdleState));

            seekIdleState.transitions.Add(new Transition(seekIdleState.IdleTimeReached, seekState));
            seekIdleState.transitions.Add(new Transition(ChaseTargetFound, chaseState));

            seekState.transitions.Add(new Transition(seekState.TargetReached, seekIdleState));
            seekState.transitions.Add(new Transition(ChaseTargetFound, chaseState));

            seekState.onEnter += SetNextSeekPosition;
            seekState.onEnter += () => onStartMoving?.Invoke();
            chaseState.onEnter += () => onStartMoving?.Invoke();
            seekIdleState.onEnter += () => onStartIdling?.Invoke();
        }

        public override void Enter()
        {
            base.Enter();
            currentState = chaseState;
            currentState.Enter();
            Debug.Log("Entered SeekAndAttack state.");
        }

        private void SetNextSeekPosition()
        {
            enemy.TargetPosition = enemy.GetRandomPointAroundCurrentWaypoint();
        }

        private bool ChaseTargetFound() => Vector3.Distance(transform.position, target.position) <= enemy.ChaseRange;
        private bool ChaseTargetLost()  => Vector3.Distance(transform.position, target.position) >  enemy.ChaseRange;
        private bool AttackTargetFound() => Vector3.Distance(transform.position, target.position) <= enemy.AttackRange;
        private bool AttackTargetLost()  => Vector3.Distance(transform.position, target.position) >  enemy.AttackRange;
    }
}
