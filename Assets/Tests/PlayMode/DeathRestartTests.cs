using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using CMGTSA.Core;
using CMGTSA.Game;
using CMGTSA.Player;

namespace CMGTSA.Tests
{
    /// <summary>
    /// PlayMode coverage of the death pipeline:
    /// PlayerStatsModel.Damage at HP 0 -> PlayerDiedEvent -> GameManager -> GameOverEvent.
    /// </summary>
    public class DeathRestartTests
    {
        [UnitySetUp]
        public IEnumerator UnitySetUp()
        {
            EventBus<PlayerHPChangedEvent>.Clear();
            EventBus<PlayerDiedEvent>.Clear();
            EventBus<GameOverEvent>.Clear();
            EventBus<GameRestartedEvent>.Clear();

            yield return SceneManager.LoadSceneAsync(
                "CombatPlayModeScene", LoadSceneMode.Single);
        }

        [UnityTearDown]
        public IEnumerator UnityTearDown()
        {
            EventBus<PlayerHPChangedEvent>.Clear();
            EventBus<PlayerDiedEvent>.Clear();
            EventBus<GameOverEvent>.Clear();
            EventBus<GameRestartedEvent>.Clear();
            yield return null;
        }

        [UnityTest]
        public IEnumerator PlayerStatsModel_damage_to_zero_publishes_PlayerDied_then_GameOver()
        {
            var gmHost = new GameObject("GM");
            gmHost.AddComponent<GameManager>();
            yield return null;       // GM subscribes in OnEnable

            int diedCalls = 0;
            int gameOverCalls = 0;
            EventBus<PlayerDiedEvent>.Subscribe(_ => diedCalls++);
            EventBus<GameOverEvent>.Subscribe(_ => gameOverCalls++);

            var stats = new PlayerStatsModel(maxHP: 3);
            stats.Damage(99);
            yield return null;

            Assert.AreEqual(1, diedCalls,     "PlayerDiedEvent fires exactly once");
            Assert.AreEqual(1, gameOverCalls, "GameManager turns the death event into GameOverEvent");
        }

        [UnityTest]
        public IEnumerator GameManager_publishes_GameRestartedEvent_on_Start()
        {
            int restartedCalls = 0;
            EventBus<GameRestartedEvent>.Subscribe(_ => restartedCalls++);

            var gmHost = new GameObject("GM");
            gmHost.AddComponent<GameManager>();
            yield return null;       // Start runs and publishes GameRestartedEvent
            yield return null;

            Assert.AreEqual(1, restartedCalls);
        }
    }
}
