using UnityEngine;
using UnityEngine.AI;
using System;

namespace CMGTSA.FSM
{
    /// <summary>
    /// FSM that controls enemy behavior when in ALERT mode.
    /// The enemy will chase, align to, attack, or seek the target depending on proximity and visibility.
    /// </summary>
    public class SeekAndAttackFSM : FSM
    {
        // FSM states
        private MoveToState chaseState;                         // Actively chase the player
        private AttackState attackState;                        // Attack
        private MoveToState seekState;                          // Move to a random seek position if player is lost
        private IdleState seekIdleState;                        // Wait briefly before seeking again

        // Events for triggering animation or VFX
        public Action onStartMoving;
        public Action onStartIdling;
        public Action onStartAttacking;

        private Transform transform;  // Dog transform
        private Transform target;     // Player transform

        /// <summary>
        /// Initializes the SeekAndAttack FSM and sets up all states and transitions.
        /// </summary>
        public SeekAndAttackFSM(NavMeshAgent navMeshAgent, ref Enemy pEnemy)
        {
            transform = pEnemy.StateOwnerTransform;
            target = pEnemy.Target;
            //blackboard = pBlackboard;
            enemy = pEnemy;

            // Create FSM states
            chaseState = new MoveToState(navMeshAgent, enemy, true);
            //alignToTargetState = new AlignToState(transform, blackboard, true);
            attackState = new AttackState(transform, target, enemy);
            seekState = new MoveToState(navMeshAgent, enemy);
            seekIdleState = new IdleState(enemy);
            //alignToNextSeekPositionState = new AlignToState(transform, blackboard);

            // --- Transitions setup ---
            // While aligning to the target:
            // 1. if attack target is found, and the dog is aligned with the target, transition to attack state.
            // 2. if attack target is lost, transition to chase state.
            //alignToTargetState.transitions.Add(new Transition(() =>
            //{
            //    return AttackTargetFound() && alignToTargetState.AlignedWithTarget();
            //}, attackState));
            //// Go back to alignment after attacking
            //alignToTargetState.transitions.Add(new Transition(AttackTargetLost, chaseState));

            attackState.transitions.Add(new Transition(attackState.FinishedAttacking, chaseState));

            chaseState.transitions.Add(new Transition(AttackTargetFound, attackState));
            chaseState.transitions.Add(new Transition(ChaseTargetLost, seekIdleState));

            seekIdleState.transitions.Add(new Transition(seekIdleState.IdleTimeReached, seekState));
            seekIdleState.transitions.Add(new Transition(ChaseTargetFound, chaseState));

            seekState.transitions.Add(new Transition(seekState.TargetReached, seekIdleState));
            seekState.transitions.Add(new Transition(ChaseTargetFound, chaseState));

            // Set random position around waypoint when entering seek alignment state
            seekState.onEnter += SetNextSeekPosition;

            // Animation/VFX event hooks
            seekState.onEnter += () => { onStartMoving?.Invoke(); };
            chaseState.onEnter += () => { onStartMoving?.Invoke(); };
            seekIdleState.onEnter += () => { onStartIdling?.Invoke(); };
        }

        // Starts the FSM in chase mode.
        public override void Enter()
        {
            base.Enter();
            currentState = chaseState;
            currentState.Enter();
            Debug.Log("Entered SeekAndAttack state.");
        }

        // Sets a new random target position around the current waypoint for seeking.
        private void SetNextSeekPosition()
        {
            blackboard.targetPosition = blackboard.GetRandomPointAroundCurrentWaypoint();
        }

        // ---------- Helper Conditions for Transitions ----------

        // Returns true if the player is in chase range.
        private bool ChaseTargetFound()
        {
            return Vector3.Distance(transform.position, target.position) <= blackboard.chaseRange;
        }

        // Returns true if the player is too far to chase.
        private bool ChaseTargetLost()
        {
            return Vector3.Distance(transform.position, target.position) > blackboard.chaseRange;
        }

        // Returns true if the player is close enough to attack.
        private bool AttackTargetFound()
        {
            return Vector3.Distance(transform.position, target.position) <= blackboard.attackRange;
        }

        // Returns true if the player moved out of attack range.
        private bool AttackTargetLost()
        {
            return Vector3.Distance(transform.position, target.position) > blackboard.attackRange;
        }
    }
}