using UnityEngine;

namespace CMGTSA.Enemies
{
    /// <summary>
    /// ScriptableObject Factory: produces a fresh <see cref="Enemy"/> instance per spawn
    /// from designer-tuned defaults.
    /// </summary>
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
    public class EnemyData : ScriptableObject
    {
        public int maxHP;
        public int money;
        public int xp;
        public Vector3 targetPosition;
        public float rotateSpeed = 180f;
        public Transform stateOwnerTransform;
        public float attackInterval = 0.5f;
        public float distanceThreshold = 0.2f;

        [SerializeField]
        private bool isRegularEnemy = true;

        public Transform target;
        public float waitingTime = 2f;
        public float attackRange = 1f;
        public float chaseRange = 2.5f;
        public float normalModeSpeed = 1f;
        public float alertModeSpeed = 2f;
        public float normalModeWaitingTime = 2f;
        public float alertModeWaitingTime = 1f;

        public Enemy CreateEnemy()
        {
            return new Enemy(maxHP, money, xp, targetPosition, rotateSpeed, stateOwnerTransform,
                attackInterval, distanceThreshold, target, attackRange, chaseRange,
                normalModeSpeed, alertModeSpeed, normalModeWaitingTime, alertModeWaitingTime,
                isRegularEnemy);
        }
    }
}
