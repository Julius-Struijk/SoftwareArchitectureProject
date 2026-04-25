using System;
using UnityEngine;

namespace CMGTSA.Enemies
{
    /// <summary>
    /// Per-instance enemy runtime data produced by <see cref="EnemyData.CreateEnemy"/>.
    /// All FSM states read this in place of the deleted Blackboard.
    /// </summary>
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
        public Vector3 TargetPosition { get; set; }
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

        /// <summary>
        /// Per-enemy alert state. Written by EnemyController on AlertLevelChangedEvent,
        /// read by EnemyFSM transitions. Replaces the removed EnemyModeController static.
        /// </summary>
        public AlertLevel alertLevel = AlertLevel.NORMAL;

        public Enemy(int pMaxHP, int pMoney, int pXP, Vector3 pTargetPosition, float pRotateSpeed,
            Transform pStateOwnerTransform, float pAttackInterval, float pDistanceThreshold,
            Transform pTarget, float pAttackRange, float pChaseRange, float pNormalModeSpeed,
            float pAlertModeSpeed, float pNormalModeWaitingTime, float pAlertModeWaitingTime,
            bool pIsRegularEnemy)
        {
            maxHP = pMaxHP;
            currentHP = pMaxHP;
            money = pMoney;
            xp = pXP;
            TargetPosition = pTargetPosition;
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
            waitingTime = pNormalModeWaitingTime;
            alertModeWaitingTime = pAlertModeWaitingTime;
            isRegularEnemy = pIsRegularEnemy;
        }

        /// <summary>
        /// Returns a random point in a unit circle around the recorded start position.
        /// Used by SeekAndAttackFSM to pick a random destination when the target is lost.
        /// Moved here from EnemyController as part of the Blackboard→Enemy refactor.
        /// </summary>
        public Vector3 GetRandomPointAroundCurrentWaypoint()
        {
            Vector2 randomOffset = UnityEngine.Random.insideUnitCircle.normalized * 2f;
            return startPosition + new Vector3(randomOffset.x, randomOffset.y, 0f);
        }
    }
}
