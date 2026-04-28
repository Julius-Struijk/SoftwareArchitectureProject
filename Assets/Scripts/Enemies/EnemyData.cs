using UnityEngine;
using CMGTSA.Battle;
using CMGTSA.Inventory;

namespace CMGTSA.Enemies
{
    /// <summary>
    /// ScriptableObject Factory: produces a fresh <see cref="Enemy"/> instance per spawn
    /// from designer-tuned defaults. Holds the outgoing <see cref="attackDamage"/> the enemy
    /// publishes via <c>EnemyAttackRequestedEvent</c>.
    /// </summary>
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
    public class EnemyData : ScriptableObject
    {
        public int maxHP;
        public int money;
        public int xp;
        public Vector3 targetPosition;
        public float rotateSpeed = 180f;
        public float attackInterval = 0.5f;
        public float distanceThreshold = 0.2f;

        [SerializeField]
        private bool isRegularEnemy = true;

        public float waitingTime = 2f;
        public float attackRange = 1f;
        public float chaseRange = 2.5f;
        public float normalModeSpeed = 1f;
        public float alertModeSpeed = 2f;
        public float normalModeWaitingTime = 2f;
        public float alertModeWaitingTime = 1f;

        [Tooltip("Outgoing damage when this enemy attacks. Inspector-assigned.")]
        public DamageData attackDamage;

        [Tooltip("Slice 3: rolled by LootDropper on EnemyDiedEvent. Each entry is independent.")]
        public LootEntry[] lootTable;

        public Enemy CreateEnemy()
        {
            return new Enemy(maxHP, money, xp, targetPosition, rotateSpeed,
                attackInterval, distanceThreshold, attackRange, chaseRange,
                normalModeSpeed, alertModeSpeed, normalModeWaitingTime, alertModeWaitingTime,
                isRegularEnemy);
        }
    }
}
