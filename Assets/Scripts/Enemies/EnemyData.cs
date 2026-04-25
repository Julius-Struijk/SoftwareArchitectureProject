using UnityEngine;
using System;

/// <summary>
/// A ScriptableObject that creates Enemy objects(Factory pattern).
/// </summary>
[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    public int maxHP;
    public int money;
    public int xp;
    public Vector3 targetPosition;           // Target position for alignment or movement
    public float rotateSpeed = 180f;         // Rotation speed in degrees per second
    public Transform stateOwnerTransform;    // The transform of the FSM owner
    public float attackInterval = 0.5f;      // Time between attacks
    public float distanceThreshold = 0.2f;   // Distance tolerance for reaching a destination

    [SerializeField]
    private bool isRegularEnemy = true;

   
    public Transform target;  // The target Transform that the enemy may chase or attack
    public float waitingTime = 2f; // Current wait time at a waypoint
    public float attackRange = 1f; // Distance within which the enemy can attack
    public float chaseRange = 2.5f; // Distance within which the enemy will start chasing the target
    public float normalModeSpeed = 1f;     // Movement speed when in normal patrol mode
    public float alertModeSpeed = 2f;     // Movement speed when in alert mode (after detecting the player)
    public float normalModeWaitingTime = 2f;     // Wait time at a waypoint when patrolling
    public float alertModeWaitingTime = 1f;     // Wait time at a waypoint when in alert mode

    public Enemy CreateEnemy()
    {
        return new Enemy(maxHP, money, xp, targetPosition, rotateSpeed, stateOwnerTransform, attackInterval, distanceThreshold, target, attackRange, chaseRange, normalModeSpeed, alertModeSpeed, normalModeWaitingTime, alertModeWaitingTime, isRegularEnemy);
    }
}

[Serializable]
public class Enemy
{
    public int MaxHP => maxHP;
    private int maxHP;
    public int currentHP;
    public int Money => money;
    private int money;
    public int XP => xp;
    private int xp;
    public Vector3 TargetPosition => targetPosition;
    private Vector3 targetPosition;
    public Vector3 startPosition;
    public float RotateSpeed => rotateSpeed;
    private float rotateSpeed;
    public Transform StateOwnerTransform => stateOwnerTransform;
    private Transform stateOwnerTransform;
    public float AttackInterval => attackInterval;
    private float attackInterval;
    public float DistanceThreshold => distanceThreshold;
    private float distanceThreshold;
    public bool IsRegularEnemy => isRegularEnemy;
    private bool isRegularEnemy;
    public Transform Target => target;
    private Transform target;

    public float waitingTime;
    public float AttackRange => attackRange;
    private float attackRange;
    public float ChaseRange => chaseRange;
    private float chaseRange;
    public float NormalModeSpeed => normalModeSpeed;
    private float normalModeSpeed;
    public float AlertModeSpeed => alertModeSpeed;
    private float alertModeSpeed;
    public float moveSpeed;
    public float NormalModeWaitingTime => normalModeWaitingTime;
    private float normalModeWaitingTime;
    public float AlertModeWaitingTime => alertModeWaitingTime;
    private float alertModeWaitingTime;

    public Enemy(int pMaxHP, int pMoney, int pXP, Vector3 pTargetPosition, float pRotateSpeed, Transform pStateOwnerTransform, float pAttackInterval, float pDistanceThreshold, Transform pTarget, float pAttackRange, float pChaseRange, float pNormalModeSpeed, float pAlertModeSpeed, float pNormalModeWaitingTime, float pAlertModeWaitingTime, bool pIsRegularEnemy)
    {
        maxHP = pMaxHP;
        currentHP = pMaxHP;
        money = pMoney;
        xp = pXP;
        targetPosition = pTargetPosition;
        rotateSpeed = pRotateSpeed;
        stateOwnerTransform = pStateOwnerTransform;
        attackInterval = pAttackInterval;
        distanceThreshold = pDistanceThreshold;
        target = pTarget;
        attackRange = pAttackRange;
        chaseRange = pChaseRange;
        normalModeSpeed = pNormalModeSpeed;
        alertModeSpeed = pAlertModeSpeed;
        moveSpeed = pNormalModeSpeed;
        normalModeWaitingTime = pNormalModeWaitingTime;
        alertModeWaitingTime = pAlertModeWaitingTime;
        isRegularEnemy = pIsRegularEnemy;
    }
}