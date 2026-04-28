using UnityEngine;
using CMGTSA.Battle;
using CMGTSA.Core;
using CMGTSA.Enemies;
using CMGTSA.Inventory;

namespace CMGTSA.Player
{
    /// <summary>
    /// Player MonoBehaviour. Owns <see cref="PlayerStatsModel"/> and <see cref="PlayerFSM"/>,
    /// implements <see cref="IDamageable"/> so <c>DamageResolver</c> can hit it, and grants
    /// XP/Money on <c>EnemyDiedEvent</c>. The FSM and the states are pure C#, no Unity refs;
    /// this class is the bridge between the framework and the engine.
    /// </summary>
    [RequireComponent(typeof(PlayerControl), typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour, IDamageable
    {
        [SerializeField] private int maxHP = 10;
        [SerializeField] private int startingMoney = 0;
        [SerializeField] private float moveSpeed = 3f;

        [Header("Attack")]
        [SerializeField] private DamageData attackDamage;
        [SerializeField] private float attackRange = 1.5f;
        [SerializeField] private float attackInterval = 0.4f;

        [Header("Hurt")]
        [SerializeField] private float hurtDuration = 0.3f;

        [Header("Inventory (slice 3)")]
        [Tooltip("ItemCategory SO for consumables (HP potions, food).")]
        [SerializeField] private ItemCategory consumableCategory;
        [Tooltip("ItemCategory SO for passive equipment (charms, rings).")]
        [SerializeField] private ItemCategory passiveCategory;
        [Tooltip("ItemCategory SO for quest items (keys, plot items).")]
        [SerializeField] private ItemCategory questCategory;

        private PlayerControl input;
        private Rigidbody2D body;
        private PlayerStatsModel stats;
        private PlayerFSM fsm;
        private InventoryModel inventory;
        private PlayerInventoryContext inventoryContext;

        public PlayerControl Input => input;
        public Rigidbody2D Body => body;
        public PlayerStatsModel Stats => stats;
        public InventoryModel Inventory => inventory;
        public Vector2 LastFacing { get; private set; } = Vector2.right;

        public DamageData AttackDamage => attackDamage;
        public float AttackRange => attackRange;
        public float AttackInterval => attackInterval;
        public float HurtDuration => hurtDuration;
        public float MoveSpeed => moveSpeed;

        private bool wantsHurt;
        public bool ConsumeHurt() { bool r = wantsHurt; wantsHurt = false; return r; }

        private void Awake()
        {
            input = GetComponent<PlayerControl>();
            body = GetComponent<Rigidbody2D>();
            stats = new PlayerStatsModel(maxHP, startingMoney);
            if (attackDamage == null) attackDamage = new DamageData { damage = 2 };
            fsm = new PlayerFSM(this);
            inventoryContext = new PlayerInventoryContext(stats);
            var registry = new ItemUseStrategyRegistry(new[]
            {
                (consumableCategory, (IItemUseStrategy)new ConsumableUseStrategy()),
                (passiveCategory,    (IItemUseStrategy)new PassiveEquipStrategy()),
                (questCategory,      (IItemUseStrategy)new QuestItemUseStrategy()),
            });
            inventory = new InventoryModel(inventoryContext, registry);
        }

        private void OnEnable()
        {
            EventBus<EnemyDiedEvent>.Subscribe(OnEnemyDied);
            EventBus<ItemPickedUpEvent>.Subscribe(OnItemPickedUp);
            fsm.Enter();
        }

        private void OnDisable()
        {
            EventBus<EnemyDiedEvent>.Unsubscribe(OnEnemyDied);
            EventBus<ItemPickedUpEvent>.Unsubscribe(OnItemPickedUp);
        }

        private void Start()
        {
            // Re-publish initial HP so HUD presenters that subscribed in their own OnEnable
            // can show the current value before the first damage tick.
            EventBus<PlayerHPChangedEvent>.Publish(new PlayerHPChangedEvent(
                stats.CurrentHP, stats.MaxHP, 0));
        }

        private void Update()
        {
            // Update facing from input. If input is zero we keep the last non-zero facing.
            if (input.MoveInput.sqrMagnitude > 0.0001f)
            {
                LastFacing = input.MoveInput.normalized;
            }
            fsm.Step();
        }

        public bool TakeDamage(int amount)
        {
            if (amount <= 0) return false;
            int prev = stats.CurrentHP;
            stats.Damage(amount);
            // Trigger the Hurt-state transition only if this was a non-fatal hit.
            // A fatal hit drops to Dead directly via the priority-1 transition.
            if (stats.CurrentHP > 0 && stats.CurrentHP < prev)
            {
                wantsHurt = true;
            }
            return stats.CurrentHP == 0;
        }

        private void OnEnemyDied(EnemyDiedEvent evt)
        {
            stats.GainXP(evt.XP);
            stats.GainMoney(evt.Money);
        }

        private void OnItemPickedUp(ItemPickedUpEvent evt)
        {
            inventory.Add(evt.Item);
        }
    }
}
