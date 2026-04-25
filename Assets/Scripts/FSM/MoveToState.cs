using UnityEngine;
using UnityEngine.AI;

namespace CMGTSA.FSM
{
    /// <summary>
    /// A state that uses Unity's NavMeshAgent to move toward a target position.
    /// Can either follow a static position or a dynamically moving target (the player).
    /// </summary>
    public class MoveToState : State
    {
        // Reference to the NavMeshAgent used for movement
        protected NavMeshAgent navMeshAgent;

        // Whether the target is moving and should be followed every frame
        private bool followMovingTarget;

        /// <summary>
        /// Constructor to initialize the MoveToState.
        /// </summary>
        /// <param name="pNavMeshAgent">The NavMeshAgent to control.</param>
        /// <param name="pBlackboard">Shared data container used by states to access and store relevant information.</param>
        /// <param name="pFollowMovingTarget">Whether to follow a moving target (true) or static position (false).</param>
        public MoveToState(NavMeshAgent pNavMeshAgent, Enemy pEnemy, bool pFollowMovingTarget = false)
        {
            navMeshAgent = pNavMeshAgent;
            //blackboard = pBlackboard;
            enemy = pEnemy;
            followMovingTarget = pFollowMovingTarget;
        }

        // Called when the state is entered.
        // Enables the NavMeshAgent and sets its speed.
        public override void Enter()
        {
            base.Enter();
            Debug.Log("Entered MoveTo state.");
            navMeshAgent.enabled = true;
            navMeshAgent.speed = enemy.moveSpeed;
        }

        // Called when the state is exited.
        // Disables the NavMeshAgent to stop movement.
        public override void Exit()
        {
            base.Exit();
            navMeshAgent.enabled = false;
        }

        // Called every frame to update movement toward the target.
        // The destination is updated only if the target is moving.
        public override void Step()
        {
            if (!followMovingTarget)
            {
                // Set destination to a static position
                navMeshAgent.SetDestination(enemy.TargetPosition);
            }
            else
            {
                // Set destination to the current position of a moving target
                navMeshAgent.SetDestination(enemy.Target.position);
            }

            base.Step();
        }

        // Checks whether the agent has reached the target (static or moving).
        // Uses the blackboard's distance threshold to determine proximity.
        public bool TargetReached()
        {
            if (!followMovingTarget)
            {
                return Vector3.Distance(navMeshAgent.transform.position, enemy.TargetPosition) < enemy.DistanceThreshold;
            }
            else
            {
                return Vector3.Distance(navMeshAgent.transform.position, enemy.Target.position) < enemy.DistanceThreshold;
            }
        }
    }
}
