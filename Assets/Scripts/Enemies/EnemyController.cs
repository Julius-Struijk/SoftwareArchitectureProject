using System;
using UnityEngine;
using UnityEngine.AI;
using CMGTSA.Battle;
using CMGTSA.Core;
using CMGTSA.FSM;

namespace CMGTSA.Enemies
{
    /// <summary>
    /// MonoBehaviour wrapper for an Enemy + EnemyFSM. Constructs both in Awake (avoids the
    /// previous OnEnable/Start NRE), subscribes to AlertLevelChangedEvent in OnEnable so the
    /// FSM transitions react to the bus, and ticks the FSM in Update.
    /// </summary>
    public class EnemyController : MonoBehaviour
    {
        [SerializeField]
        private EnemyData enemyData;

        [SerializeField]
        private NavMeshAgent navMeshAgent;

        private Enemy enemy;
        private EnemyFSM enemyFSM;

        public event Action<Enemy> onEnemyCreated;
        public event Action<Enemy, DamageData> onHit;

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

        public void GetHit(DamageData damageData)
        {
            enemy.currentHP -= damageData.damage;
            if (enemy.currentHP < 0) enemy.currentHP = 0;
            onHit?.Invoke(enemy, damageData);
        }
    }
}
