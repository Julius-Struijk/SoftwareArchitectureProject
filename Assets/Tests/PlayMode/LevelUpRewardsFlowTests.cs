using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using CMGTSA.Core;
using CMGTSA.Player;

namespace CMGTSA.Tests
{
    /// <summary>
    /// PlayMode regression: a real level-up via PlayerStatsModel.GainXP must
    /// flow through PlayerController.OnLeveledUp, look up the rewards table,
    /// and bump MaxHP + CurrentAttackDamage on the model. EditMode covers
    /// the model and the table independently; this test wires them together
    /// via a real PlayerController instance.
    /// </summary>
    public class LevelUpRewardsFlowTests
    {
        private LevelUpRewardsTable rewardsTable;

        [UnitySetUp]
        public IEnumerator UnitySetUp()
        {
            EventBus<PlayerHPChangedEvent>.Clear();
            EventBus<PlayerXPGainedEvent>.Clear();
            EventBus<PlayerLeveledUpEvent>.Clear();
            EventBus<PlayerDiedEvent>.Clear();

            // Use the existing combat scene so PlayerController has a Rigidbody2D and PlayerControl.
            yield return SceneManager.LoadSceneAsync(
                "CombatPlayModeScene", LoadSceneMode.Single);

            rewardsTable = ScriptableObject.CreateInstance<LevelUpRewardsTable>();
            rewardsTable.rewards = new[]
            {
                new LevelUpRewardsTable.LevelReward { level = 2, hpDelta = 2, damageDelta = 1, healToFull = true },
            };
            rewardsTable.fallback = new LevelUpRewardsTable.LevelReward
            {
                level = 0, hpDelta = 0, damageDelta = 0, healToFull = false,
            };
        }

        [UnityTearDown]
        public IEnumerator UnityTearDown()
        {
            if (rewardsTable != null) Object.DestroyImmediate(rewardsTable);

            EventBus<PlayerHPChangedEvent>.Clear();
            EventBus<PlayerXPGainedEvent>.Clear();
            EventBus<PlayerLeveledUpEvent>.Clear();
            EventBus<PlayerDiedEvent>.Clear();
            yield return null;
        }

        [UnityTest]
        public IEnumerator GainXP_pastThreshold_appliesRewards_andHealsToFull()
        {
            var player = Object.FindFirstObjectByType<PlayerController>();
            Assert.IsNotNull(player, "CombatPlayModeScene must contain a PlayerController.");

            // Inject our fixed rewards table via reflection so we don't depend on
            // whatever the scene's player has assigned.
            SetPrivateField(player, "rewardsTable", rewardsTable);

            var stats = player.Stats;
            Assert.IsNotNull(stats, "PlayerStatsModel must be initialized in Awake.");

            int initialMaxHP = stats.MaxHP;
            int initialDamage = stats.CurrentAttackDamage;

            // Damage the player so heal-to-full is observable (drop CurrentHP below MaxHP).
            stats.Damage(1);
            Assert.AreEqual(initialMaxHP - 1, stats.CurrentHP, "Pre-condition: player should have taken 1 damage.");

            // Cross level 1 -> 2: needs 5 XP (5 * level).
            stats.GainXP(5);

            yield return null;

            Assert.AreEqual(2, stats.Level, "Player should be at level 2.");
            Assert.AreEqual(initialMaxHP + 2, stats.MaxHP,
                "MaxHP should be up by the table's hpDelta (+2).");
            Assert.AreEqual(initialDamage + 1, stats.CurrentAttackDamage,
                "CurrentAttackDamage should be up by the table's damageDelta (+1).");
            Assert.AreEqual(stats.MaxHP, stats.CurrentHP,
                "healToFull should bring CurrentHP up to the new MaxHP.");
        }

        private static void SetPrivateField(object target, string name, object value)
        {
            FieldInfo f = target.GetType().GetField(name,
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(f, $"Field {name} not found on {target.GetType().Name}");
            f.SetValue(target, value);
        }
    }
}
