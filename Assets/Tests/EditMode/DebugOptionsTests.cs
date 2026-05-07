using NUnit.Framework;
using CMGTSA.Core;
using CMGTSA.Game;
using CMGTSA.Player;

namespace CMGTSA.Tests
{
    public class DebugOptionsTests
    {
        [SetUp]
        public void SetUp()
        {
            EventBus<PlayerHPChangedEvent>.Clear();
            EventBus<PlayerXPGainedEvent>.Clear();
            EventBus<PlayerLeveledUpEvent>.Clear();
            EventBus<PlayerDiedEvent>.Clear();
            EventBus<DebugCheatToggledEvent>.Clear();
            DebugOptions.GodMode = false;
        }

        [TearDown]
        public void TearDown()
        {
            EventBus<PlayerHPChangedEvent>.Clear();
            EventBus<PlayerXPGainedEvent>.Clear();
            EventBus<PlayerLeveledUpEvent>.Clear();
            EventBus<PlayerDiedEvent>.Clear();
            EventBus<DebugCheatToggledEvent>.Clear();
            DebugOptions.GodMode = false;
        }

        [Test]
        public void GiveXP_cheat_magnitude_lands_on_PlayerStatsModel()
        {
            var stats = new PlayerStatsModel(maxHP: 10);
            int receivedXP = 0;
            EventBus<PlayerXPGainedEvent>.Subscribe(e => receivedXP = e.Gained);

            stats.GainXP(DebugOptions.GiveXPAmount);

            Assert.AreEqual(DebugOptions.GiveXPAmount, receivedXP);
        }

        [Test]
        public void GiveMoney_cheat_magnitude_lands_on_PlayerStatsModel()
        {
            var stats = new PlayerStatsModel(maxHP: 10, startingMoney: 0);

            stats.GainMoney(DebugOptions.GiveMoneyAmount);

            Assert.AreEqual(DebugOptions.GiveMoneyAmount, stats.Money);
        }

        [Test]
        public void TriggerGameOver_cheat_drops_HP_to_zero_and_publishes_PlayerDied()
        {
            var stats = new PlayerStatsModel(maxHP: 10);
            int diedCount = 0;
            EventBus<PlayerDiedEvent>.Subscribe(_ => diedCount++);

            stats.Damage(99999);

            Assert.AreEqual(0, stats.CurrentHP);
            Assert.AreEqual(1, diedCount);
        }

        [Test]
        public void GodMode_default_is_false()
        {
            Assert.IsFalse(DebugOptions.GodMode);
        }
    }
}
