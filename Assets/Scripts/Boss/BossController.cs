using UnityEngine;
using UnityEngine.AI;
using CMGTSA.Core;

namespace CMGTSA.Boss
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class BossController : MonoBehaviour, IDamageable
    {
        [SerializeField] private BossData bossData;
        [SerializeField] private NavMeshAgent agent;

        private Transform playerTransform;
        private BossHealthModel health;
        private BossPhaseSelector selector;
        private BossPatternContext patternContext;
        private BossFSM fsm;
        private bool encounterActive;
        private int patternRotationIndex;

        public BossData Data => bossData;
        public BossHealthModel Health => health;

        private void Awake()
        {
            if (agent == null) agent = GetComponent<NavMeshAgent>();
            if (agent != null && agent.enabled)
            {
                agent.updateRotation = false;
                agent.updateUpAxis = false;
                agent.speed = bossData != null ? bossData.moveSpeed : 2f;
            }

            GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
            playerTransform = playerGO != null ? playerGO.transform : null;

            health = new BossHealthModel(bossData != null ? bossData.maxHP : 100, 0);
            selector = new BossPhaseSelector(bossData != null ? bossData.phases : null);
            patternContext = new BossPatternContext(transform, playerTransform);

            BossIdleState idle = new BossIdleState(() => encounterActive);
            BossChaseState chase = new BossChaseState(transform, playerTransform, agent,
                () => CurrentPhase() != null ? CurrentPhase().castIntervalSeconds : 2f,
                () => bossData != null ? bossData.engagementRange : 4f);
            BossCastPatternState cast = new BossCastPatternState(NextPatternRuntime, () => patternContext);
            BossDeadState dead = new BossDeadState(() => bossData, transform, OnBossDestroyed);

            fsm = new BossFSM(idle, chase, cast, dead, () => health.Current == 0);
        }

        private void OnEnable()
        {
            EventBus<BossEncounterStartedEvent>.Subscribe(OnEncounterStarted);
            fsm.Enter();
        }

        private void OnDisable()
        {
            EventBus<BossEncounterStartedEvent>.Unsubscribe(OnEncounterStarted);
        }

        private void Start()
        {
            EventBus<BossHPChangedEvent>.Publish(new BossHPChangedEvent(
                health.Current, health.Max, health.PhaseIndex));
        }

        private void Update()
        {
            fsm.Step();
        }

        public bool TakeDamage(int amount)
        {
            if (amount <= 0 || health.Current <= 0) return false;
            bool dead = health.Damage(amount);

            int newPhase = selector.SelectPhase(health.Fraction);
            if (newPhase != health.PhaseIndex)
            {
                health.SetPhaseIndex(newPhase);
                patternRotationIndex = 0;
                EventBus<BossHPChangedEvent>.Publish(new BossHPChangedEvent(
                    health.Current, health.Max, health.PhaseIndex));
            }
            return dead;
        }

        private void OnEncounterStarted(BossEncounterStartedEvent _)
        {
            encounterActive = true;
        }

        private BossPhase CurrentPhase() => selector.GetPhase(health.PhaseIndex);

        private IBossPatternRuntime NextPatternRuntime()
        {
            BossPhase phase = CurrentPhase();
            if (phase == null || phase.patterns == null || phase.patterns.Length == 0) return null;
            int i = patternRotationIndex % phase.patterns.Length;
            patternRotationIndex++;
            SOBossAttackPattern pattern = phase.patterns[i];
            return pattern != null ? pattern.CreateRuntime() : null;
        }

        private void OnBossDestroyed()
        {
            Destroy(gameObject, 0.1f);
        }
    }
}
