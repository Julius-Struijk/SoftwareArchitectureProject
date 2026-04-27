using NUnit.Framework;
using CMGTSA.Core;
using CMGTSA.Player;

namespace CMGTSA.Tests
{
    public class PlayerStatsModelTests
    {
        [SetUp]
        public void SetUp()
        {
            EventBus<PlayerHPChangedEvent>.Clear();
            EventBus<PlayerXPGainedEvent>.Clear();
            EventBus<PlayerLeveledUpEvent>.Clear();
            EventBus<PlayerDiedEvent>.Clear();
        }

        [TearDown]
        public void TearDown()
        {
            EventBus<PlayerHPChangedEvent>.Clear();
            EventBus<PlayerXPGainedEvent>.Clear();
            EventBus<PlayerLeveledUpEvent>.Clear();
            EventBus<PlayerDiedEvent>.Clear();
        }

        [Test]
        public void Constructor_initializes_with_full_HP_level_one_zero_xp()
        {
            var stats = new PlayerStatsModel(maxHP: 10, startingMoney: 5);

            Assert.AreEqual(10, stats.MaxHP);
            Assert.AreEqual(10, stats.CurrentHP);
            Assert.AreEqual(1,  stats.Level);
            Assert.AreEqual(0,  stats.XP);
            Assert.AreEqual(5,  stats.Money);
        }

        [Test]
        public void Damage_reduces_HP_and_publishes_HPChanged_with_negative_delta()
        {
            var stats = new PlayerStatsModel(10);
            int receivedDelta = 0;
            int receivedCurrent = -1;
            void Handler(PlayerHPChangedEvent e) { receivedDelta = e.Delta; receivedCurrent = e.CurrentHP; }
            EventBus<PlayerHPChangedEvent>.Subscribe(Handler);

            stats.Damage(3);

            Assert.AreEqual(7, stats.CurrentHP);
            Assert.AreEqual(-3, receivedDelta);
            Assert.AreEqual(7,  receivedCurrent);
        }

        [Test]
        public void Damage_clamps_to_zero_and_publishes_PlayerDiedEvent_once()
        {
            var stats = new PlayerStatsModel(5);
            int diedCalls = 0;
            EventBus<PlayerDiedEvent>.Subscribe(_ => diedCalls++);

            stats.Damage(99);

            Assert.AreEqual(0, stats.CurrentHP);
            Assert.AreEqual(1, diedCalls);
        }

        [Test]
        public void Damage_after_death_is_a_noop()
        {
            var stats = new PlayerStatsModel(5);
            stats.Damage(99);

            int diedCalls = 0;
            int hpEvents = 0;
            EventBus<PlayerDiedEvent>.Subscribe(_ => diedCalls++);
            EventBus<PlayerHPChangedEvent>.Subscribe(_ => hpEvents++);

            stats.Damage(1);

            Assert.AreEqual(0, stats.CurrentHP);
            Assert.AreEqual(0, diedCalls, "Died fires once, not on every post-mortem hit");
            Assert.AreEqual(0, hpEvents,  "HP event does not re-fire when already at 0");
        }

        [Test]
        public void Heal_increases_HP_clamped_to_max_and_publishes_HPChanged()
        {
            var stats = new PlayerStatsModel(10);
            stats.Damage(7);                   // CurrentHP = 3
            int receivedDelta = 0;
            EventBus<PlayerHPChangedEvent>.Subscribe(e => receivedDelta = e.Delta);

            stats.Heal(99);

            Assert.AreEqual(10, stats.CurrentHP);
            Assert.AreEqual(7,  receivedDelta);
        }

        [Test]
        public void Heal_after_death_is_a_noop()
        {
            var stats = new PlayerStatsModel(5);
            stats.Damage(99);
            int hpEvents = 0;
            EventBus<PlayerHPChangedEvent>.Subscribe(_ => hpEvents++);

            stats.Heal(99);

            Assert.AreEqual(0, stats.CurrentHP);
            Assert.AreEqual(0, hpEvents);
        }

        [Test]
        public void GainXP_publishes_XPGained_with_total_and_amount()
        {
            var stats = new PlayerStatsModel(10);
            int total = -1, gained = -1;
            EventBus<PlayerXPGainedEvent>.Subscribe(e => { total = e.TotalXP; gained = e.Gained; });

            stats.GainXP(3);

            Assert.AreEqual(3, stats.XP);
            Assert.AreEqual(3, total);
            Assert.AreEqual(3, gained);
        }

        [Test]
        public void GainXP_crosses_threshold_and_levels_up_once()
        {
            // Threshold formula in PlayerStatsModel: XP for next level = 5 * Level.
            // From level 1 with 0 XP, next level requires 5 XP total.
            var stats = new PlayerStatsModel(10);
            int levelUps = 0;
            int newLevel = -1;
            EventBus<PlayerLeveledUpEvent>.Subscribe(e => { levelUps++; newLevel = e.NewLevel; });

            stats.GainXP(5);

            Assert.AreEqual(2, stats.Level);
            Assert.AreEqual(1, levelUps);
            Assert.AreEqual(2, newLevel);
        }

        [Test]
        public void GainXP_levels_up_multiple_times_in_one_call()
        {
            // Level 1->2 needs 5 XP, then level 2->3 needs 10 XP more (5*2). Total 15 XP -> level 3.
            var stats = new PlayerStatsModel(10);
            int levelUps = 0;
            EventBus<PlayerLeveledUpEvent>.Subscribe(_ => levelUps++);

            stats.GainXP(15);

            Assert.AreEqual(3, stats.Level);
            Assert.AreEqual(2, levelUps);
        }

        [Test]
        public void GainMoney_increments_total_no_event()
        {
            var stats = new PlayerStatsModel(10, startingMoney: 1);
            stats.GainMoney(4);
            Assert.AreEqual(5, stats.Money);
        }

        [Test]
        public void Negative_or_zero_gain_calls_are_noops()
        {
            var stats = new PlayerStatsModel(10);
            int hpEvents = 0, xpEvents = 0;
            EventBus<PlayerHPChangedEvent>.Subscribe(_ => hpEvents++);
            EventBus<PlayerXPGainedEvent>.Subscribe(_ => xpEvents++);

            stats.Damage(0);   stats.Damage(-3);
            stats.Heal(0);     stats.Heal(-3);
            stats.GainXP(0);   stats.GainXP(-3);
            stats.GainMoney(0); stats.GainMoney(-3);

            Assert.AreEqual(10, stats.CurrentHP);
            Assert.AreEqual(0,  stats.XP);
            Assert.AreEqual(0,  stats.Money);
            Assert.AreEqual(0,  hpEvents);
            Assert.AreEqual(0,  xpEvents);
        }
    }
}
