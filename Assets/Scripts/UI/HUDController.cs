using UnityEngine;
using CMGTSA.Core;
using CMGTSA.Game;

namespace CMGTSA.UI
{
    /// <summary>
    /// Root HUD root: hides the HUD on <see cref="GameOverEvent"/> and shows it on
    /// <see cref="GameRestartedEvent"/>. The HP-bar and HP-number presenters live as
    /// children and own their own subscriptions — this controller is just visibility.
    /// </summary>
    public class HUDController : MonoBehaviour
    {
        [SerializeField] private GameObject hudRoot;

        private void OnEnable()
        {
            EventBus<GameOverEvent>.Subscribe(OnGameOver);
            EventBus<GameRestartedEvent>.Subscribe(OnGameRestarted);
        }

        private void OnDisable()
        {
            EventBus<GameOverEvent>.Unsubscribe(OnGameOver);
            EventBus<GameRestartedEvent>.Unsubscribe(OnGameRestarted);
        }

        private void OnGameOver(GameOverEvent _)     { if (hudRoot != null) hudRoot.SetActive(false); }
        private void OnGameRestarted(GameRestartedEvent _) { if (hudRoot != null) hudRoot.SetActive(true); }
    }
}
