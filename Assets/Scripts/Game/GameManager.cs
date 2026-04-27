using UnityEngine;
using UnityEngine.SceneManagement;
using CMGTSA.Core;
using CMGTSA.Player;

namespace CMGTSA.Game
{
    /// <summary>
    /// Scene-level orchestrator. Subscribes to <see cref="PlayerDiedEvent"/>, turns it into
    /// <see cref="GameOverEvent"/>, and exposes a <see cref="ReloadScene"/> method that
    /// <c>GameOverUI</c> calls when the player presses Restart. Also publishes
    /// <see cref="GameRestartedEvent"/> in <c>Start</c> so other systems can reset.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        private void OnEnable()
        {
            EventBus<PlayerDiedEvent>.Subscribe(OnPlayerDied);
        }

        private void OnDisable()
        {
            EventBus<PlayerDiedEvent>.Unsubscribe(OnPlayerDied);
        }

        private void Start()
        {
            EventBus<GameRestartedEvent>.Publish(default);
        }

        private void OnPlayerDied(PlayerDiedEvent _)
        {
            EventBus<GameOverEvent>.Publish(default);
        }

        public void ReloadScene()
        {
            int sceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(sceneIndex);
        }
    }
}
