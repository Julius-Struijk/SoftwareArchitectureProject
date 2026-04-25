using UnityEngine;
using UnityEngine.AI;
using System;

namespace CMGTSA.FSM
{
    using UnityEngine;
    using UnityEngine.AI;
    using System;

    /// <summary>
    /// PatrolFSM controls patrolling behavior for dogs in Normal mode.
    /// The FSM cycles through waypoints, idles, aligns to the next direction, then moves again.
    /// </summary>
    public class PatrolFSM : FSM
    {
        // FSM states
        private MoveToState moveToState;
        private IdleState idleState;

        // Events for triggering animation or VFX
        public Action onStartMoving;
        public Action onStartIdling;

        /// <summary>
        /// Initializes the PatrolFSM and its states with transitions between them.
        /// </summary>
        /// <param name="navMeshAgent">The NavMeshAgent controlling movement.</param>
        /// <param name="pBlackboard">Shared data container used by states to access and store relevant information.</param>
        public PatrolFSM(NavMeshAgent navMeshAgent, Enemy pEnemy)
        {
            enemy = pEnemy;
            //blackboard = pBlackboard;

            // Create FSM states
            moveToState = new MoveToState(navMeshAgent, enemy);
            idleState = new IdleState(enemy);

            // --- Transitions setup ---
            idleState.transitions.Add(new Transition(idleState.IdleTimeReached, moveToState));

            moveToState.transitions.Add(new Transition(moveToState.TargetReached, idleState));

            // Once movement starts select the waypoint to move to.
            moveToState.onEnter += SetNextSeekPosition;

            // Hook up animation/VFX events
            moveToState.onEnter += StartMoving;
            idleState.onEnter += StartIdling;
        }

        /// <summary>
        /// Called when this FSM becomes active.
        /// Starts with the MoveToState.
        /// </summary>
        public override void Enter()
        {
            base.Enter();
            Debug.Log("Entered Patrol state.");
            currentState = moveToState;
            currentState.Enter();
        }

        /// <summary>
        /// Called when movement starts. Triggers external listeners (e.g., animations).
        /// </summary>
        private void StartMoving()
        {
            onStartMoving?.Invoke();
        }

        /// <summary>
        /// Called when idling starts. Triggers external listeners (e.g., animations).
        /// </summary>
        private void StartIdling()
        {
            onStartIdling?.Invoke();
        }

        /// <summary>
        /// Called after finishing movement.
        /// Advances the waypoint index and updates the blackboard with new target and wait time.
        /// </summary>
        private void SetNextSeekPosition()
        {
            // Update blackboard with new target
            //enemy.targetPosition = enemy.GetRandomPointAroundCurrentWaypoint();
        }
    }
}