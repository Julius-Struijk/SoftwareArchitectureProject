using UnityEngine;

namespace CMGTSA.FSM
{
    /// <summary>
    /// Shared data container (Blackboard) used by FSM states to access and store relevant information.
    /// The region "For enemies only" contains logic and data specific to enemy AI behavior.
    /// </summary>
    public class Blackboard : MonoBehaviour
    {
        public Vector3 startPosition;
        public Vector3 targetPosition;           // Target position for alignment or movement
        public float moveSpeed;                  // Movement speed of the entity
        public float rotateSpeed = 180f;         // Rotation speed in degrees per second
        public Transform stateOwnerTransform;    // The transform of the FSM owner
        public float attackInterval = 0.5f;      // Time between attacks
        public float distanceThreshold = 0.2f;   // Distance tolerance for reaching a destination

        #region "For regular enemy's only"
        [SerializeField]
        private bool isRegularEnemy = true;

        // The target Transform that the dog may chase or attack
        public Transform target;

        // Wait time at a waypoint when patrolling
        public float normalModeWaitingTime = 2f;

        // Current wait time at a waypoint
        public float waitingTime = 2f;

        // Distance within which the enemy can attack
        public float attackRange = 1f;

        // Distance within which the enemy will start chasing the target
        public float chaseRange = 2.5f;

        // Movement speed when in normal patrol mode
        public float normalModeSpeed = 1f;

        // Movement speed when in alert mode (after detecting the player)
        public float alertModeSpeed = 2f;

        // Wait time at a waypoint when in alert mode
        public float alertModeWaitingTime = 1f;

        // Subscribe to enemy mode change events when the component is enabled.
        private void OnEnable()
        {
            EnemyModeController.onEnemyModeChanged += OnModeChanged;
            startPosition = gameObject.transform.position;
        }

        // Unsubscribe from enemy mode change events when the component is disabled.
        private void OnDisable()
        {
            EnemyModeController.onEnemyModeChanged -= OnModeChanged;
        }

        // Called when the enemy's mode is changed (e.g., from NORMAL to ALERT).
        // Adjusts movement speed and waiting time accordingly.
        private void OnModeChanged(EnemyMode enemyMode)
        {
            if (isRegularEnemy)
            {
                // If the enemy becomes alert, increase speed and set shorter wait time
                if (enemyMode == EnemyMode.ALERT)
                {
                    moveSpeed = alertModeSpeed;
                    waitingTime = alertModeWaitingTime;
                }
                // If the enemy returns to normal, use default speed and waypoint-specific wait time
                else
                {
                    moveSpeed = normalModeSpeed;
                    waitingTime = normalModeWaitingTime;
                }
            }
        }

        /// <summary>
        /// Returns a random point near the current waypoint.
        /// Used when enemies are in Alert mode and seeking the player(move to a random target
        /// to try to detect the player).

        public Vector3 GetRandomPointAroundCurrentWaypoint()
        {
            // Generate a random offset in a 2D circle and scale it
            Vector2 randomOffset = UnityEngine.Random.insideUnitCircle.normalized * 2f;

            // Apply the offset to the current waypoint (on the XY plane)
            return startPosition +
                new Vector3(randomOffset.x, randomOffset.y, 0f);
        }
        #endregion
    }
}