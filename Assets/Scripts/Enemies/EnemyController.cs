using System;
using UnityEngine;
using UnityEngine.AI;
using CMGTSA.Battle;
using CMGTSA.Core;
using CMGTSA.FSM;

namespace CMGTSA.Enemies
{
    /// <summary>
    /// MonoBehaviour wrapper for an Enemy + EnemyFSM. Implements <see cref="IDamageable"/> so
    /// <c>DamageResolver</c> can hit it. Publishes <see cref="EnemyAttackRequestedEvent"/>
    /// when the FSM enters its AttackState, and <see cref="EnemyDiedEvent"/> when HP hits 0.
    /// Exposes a per-instance <see cref="onHPChanged"/> C# event for the world-space HP bar
    /// (Observer pattern, intra-system one-to-many — bus would need transform filtering).
    /// </summary>
    public class EnemyController : MonoBehaviour, IDamageable
    {
        [SerializeField]
        private EnemyData enemyData;

        [SerializeField]
        private NavMeshAgent navMeshAgent;

        private Enemy enemy;
        private EnemyFSM enemyFSM;

        public event Action<Enemy> onEnemyCreated;
        public event Action<int, int> onHPChanged;     // currentHP, maxHP

        public Enemy Enemy => enemy;
        public EnemyData Data => enemyData;

        private void Awake()
        {
            if (navMeshAgent == null)
            {
                navMeshAgent = GetComponent<NavMeshAgent>();
            }
            navMeshAgent.updateRotation = false;
            navMeshAgent.updateUpAxis = false;

            enemy = enemyData.CreateEnemy();
            enemy.startPosition = transform.position;
            enemy.target = GameObject.FindGameObjectWithTag("Player").transform;
            enemy.stateOwnerTransform = transform;
            enemyFSM = new EnemyFSM(navMeshAgent, enemy);

            enemyFSM.seekAndAttackFSM.onStartAttacking += PublishAttackRequest;
        }

        private void OnEnable()
        {
            EventBus<AlertLevelChangedEvent>.Subscribe(OnAlertLevelChanged);
            enemyFSM.Enter();
        }

        private void OnDisable()
        {
            EventBus<AlertLevelChangedEvent>.Unsubscribe(OnAlertLevelChanged);
        }

        private void Start()
        {
            // Raised here (not Awake) so subscribers that subscribe in their own OnEnable
            // are guaranteed to be registered before this fires.
            onEnemyCreated?.Invoke(enemy);
            onHPChanged?.Invoke(enemy.currentHP, enemy.MaxHP);
        }

        private void Update()
        {
            enemyFSM.Step();
        }

        private void OnAlertLevelChanged(AlertLevelChangedEvent evt)
        {
            enemy.alertLevel = evt.Level;

            if (!enemy.IsRegularEnemy) return;

            if (evt.Level == AlertLevel.ALERT)
            {
                enemy.moveSpeed   = enemy.AlertModeSpeed;
                enemy.waitingTime = enemy.AlertModeWaitingTime;
            }
            else
            {
                enemy.moveSpeed   = enemy.NormalModeSpeed;
                enemy.waitingTime = enemy.NormalModeWaitingTime;
            }
        }

        public bool TakeDamage(int amount)
        {
            if (amount <= 0 || enemy.currentHP <= 0) return false;

            enemy.currentHP = Mathf.Max(0, enemy.currentHP - amount);
            onHPChanged?.Invoke(enemy.currentHP, enemy.MaxHP);

            if (enemy.currentHP == 0)
            {
                EventBus<EnemyDiedEvent>.Publish(new EnemyDiedEvent(
                    enemy.XP, enemy.Money, transform.position, enemyData));
                Destroy(gameObject);
                return true;
            }
            return false;
        }

        private void PublishAttackRequest()
        {
            EventBus<EnemyAttackRequestedEvent>.Publish(new EnemyAttackRequestedEvent(
                transform.position,
                enemy.AttackRange,
                enemyData.attackDamage,
                transform));
        }
    }
}
