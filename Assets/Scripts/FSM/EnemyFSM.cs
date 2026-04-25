using UnityEngine;
using UnityEngine.AI;

namespace CMGTSA.FSM
{
    /// <summary>
    /// A high-level finite state machine that controls a enemy's behavior.
    /// It switches between two sub-state machines: patrolling and seeking/attacking,
    /// based on the current enemy mode (NORMAL or ALERT).
    /// </summary>
    public class EnemyFSM : FSM
    {
        // Sub-FSM for patrolling behavior
        public PatrolFSM patrolFSM;

        // Sub-FSM for seeking and attacking a target
        public SeekAndAttackFSM seekAndAttackFSM;

        /// <summary>
        /// Constructor that initializes the sub-state machines and sets up transitions.
        /// </summary>
        /// <param name="navMeshAgent">The NavMeshAgent used for movement.</param>
        /// <param name="blackboard">Shared data container used by states to access and store relevant information.</param>
        public EnemyFSM(NavMeshAgent navMeshAgent, Enemy enemy)
        {
            // Initialize patrol and attack sub-state machines
            patrolFSM = new PatrolFSM(navMeshAgent, enemy);
            seekAndAttackFSM = new SeekAndAttackFSM(navMeshAgent, ref enemy);

            // Transition from patrol to seek-and-attack when the enemy enters ALERT mode
            patrolFSM.transitions.Add(new Transition(() => EnemyModeController.enemyMode == EnemyMode.ALERT, seekAndAttackFSM));

            // Transition from seek-and-attack to patrol when the enemy returns to NORMAL mode
            seekAndAttackFSM.transitions.Add(new Transition(() => EnemyModeController.enemyMode == EnemyMode.NORMAL, patrolFSM));

            // Start in patrol mode by default
            currentState = patrolFSM;
        }

        // Called when this FSM becomes active. Triggers the Enter method on the current sub-FSM.
        public override void Enter()
        {
            base.Enter();
            Debug.Log("Enemy FSM is active.");
            currentState.Enter();
        }
    }
}
