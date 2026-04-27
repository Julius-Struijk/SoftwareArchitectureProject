using UnityEngine;
using UnityEngine.InputSystem;
using CMGTSA.Core;
using CMGTSA.Game;

namespace CMGTSA.UI
{
    /// <summary>
    /// Game-over overlay panel. Uses a CanvasGroup for visibility so this MonoBehaviour
    /// stays active (and subscribed to events) even when the panel is hidden. Activates
    /// on <see cref="GameOverEvent"/>; R-key reloads the scene via <see cref="GameManager"/>.
    /// </summary>
    public class GameOverUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup panelGroup;
        [SerializeField] private GameManager gameManager;

        private bool visible;

        private void Awake()
        {
            SetPanelVisible(false);
        }

        private void OnEnable()
        {
            EventBus<GameOverEvent>.Subscribe(OnGameOver);
        }

        private void OnDisable()
        {
            EventBus<GameOverEvent>.Unsubscribe(OnGameOver);
        }

        private void Update()
        {
            if (!visible) return;
            if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
            {
                if (gameManager != null) gameManager.ReloadScene();
            }
        }

        private void OnGameOver(GameOverEvent _) => SetPanelVisible(true);

        private void SetPanelVisible(bool show)
        {
            visible = show;
            if (panelGroup == null) return;
            panelGroup.alpha = show ? 1f : 0f;
            panelGroup.interactable = show;
            panelGroup.blocksRaycasts = show;
        }
    }
}
