using UnityEngine;
using System;
using UnityEngine.AI;
using CMGTSA.FSM;

/// <summary>
/// Simple enemy controller that publish onEnemyCreated and onHit events when
/// it's created and hit.
/// </summary>
public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private EnemyData enemyData;
    private Enemy enemy;

    public event Action<Enemy> onEnemyCreated;
    public event Action<Enemy, DamageData> onHit;

    [SerializeField]
    private NavMeshAgent navMeshAgent;
    //[SerializeField]
    //private Blackboard blackboard; // Shared data container (Blackboard) used by FSM states to access and store relevant information.

    private EnemyFSM enemyFSM; // The main FSM controlling enemy behavior

    // Initializes the EnemyFSM and subscribes to its events for animations and VFX.
    private void OnEnable()
    {
        EnemyModeController.onEnemyModeChanged += OnModeChanged;
        enemy.startPosition = gameObject.transform.position;

        // Ensure navMeshAgent is assigned
        if (navMeshAgent == null)
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        // Initialize EnemyFSM if not already created
        if (enemyFSM == null)
        {
            enemyFSM = new EnemyFSM(navMeshAgent, enemy);
        }

        // Start the FSM
        enemyFSM.Enter();
    }

    // Unsubscribe from enemy mode change events when the component is disabled.
    private void OnDisable()
    {
        EnemyModeController.onEnemyModeChanged -= OnModeChanged;
    }

    void Start()
    {
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        enemy = enemyData.CreateEnemy();
        onEnemyCreated?.Invoke(enemy);
    }

    /// Advances the EnemyFSM every frame.
    void Update()
    {
        enemyFSM.Step();
    }

    // Called when the enemy's mode is changed (e.g., from NORMAL to ALERT).
    // Adjusts movement speed and waiting time accordingly.
    private void OnModeChanged(EnemyMode enemyMode)
    {
        if (enemy.IsRegularEnemy)
        {
            // If the enemy becomes alert, increase speed and set shorter wait time
            if (enemyMode == EnemyMode.ALERT)
            {
                enemy.moveSpeed = enemy.AlertModeSpeed;
                enemy.waitingTime = enemy.AlertModeWaitingTime;
            }
            // If the enemy returns to normal, use default speed and waypoint-specific wait time
            else
            {
                enemy.moveSpeed = enemy.NormalModeSpeed;
                enemy.waitingTime = enemy.NormalModeWaitingTime;
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
        return enemy.startPosition +
            new Vector3(randomOffset.x, randomOffset.y, 0f);
    }

    public void GetHit(DamageData damageData)
    {
        enemy.currentHP -= damageData.damage;
        if (enemy.currentHP < 0)
        {
            enemy.currentHP = 0;
        }

        onHit?.Invoke(enemy, damageData);
    }
}