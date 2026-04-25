using UnityEngine;
using UnityEngine.AI;
using CMGTSA.Enemies;

namespace CMGTSA.FSM
{
    /// <summary>
    /// Walks the NavMeshAgent toward either a static <see cref="Enemy.TargetPosition"/>
    /// or, when <c>followMovingTarget</c> is true, the live <see cref="Enemy.Target"/> transform.
    /// </summary>
    public class MoveToState : State
    {
        protected NavMeshAgent navMeshAgent;
        private bool followMovingTarget;

        public MoveToState(NavMeshAgent pNavMeshAgent, Enemy pEnemy, bool pFollowMovingTarget = false)
        {
            navMeshAgent = pNavMeshAgent;
            enemy = pEnemy;
            followMovingTarget = pFollowMovingTarget;
        }

        public override void Enter()
        {
            base.Enter();
            Debug.Log("Entered MoveTo state.");
            navMeshAgent.enabled = true;
            navMeshAgent.speed = enemy.moveSpeed;
        }

        public override void Exit()
        {
            base.Exit();
            navMeshAgent.enabled = false;
        }

        public override void Step()
        {
            if (!followMovingTarget)
            {
                navMeshAgent.SetDestination(enemy.TargetPosition);
            }
            else
            {
                navMeshAgent.SetDestination(enemy.target.position);
            }
            base.Step();
        }

        public bool TargetReached()
        {
            if (!followMovingTarget)
            {
                return Vector3.Distance(navMeshAgent.transform.position, enemy.TargetPosition) < enemy.DistanceThreshold;
            }
            return Vector3.Distance(navMeshAgent.transform.position, enemy.target.position) < enemy.DistanceThreshold;
        }
    }
}
