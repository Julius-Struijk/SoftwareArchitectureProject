using UnityEngine;
using UnityEngine.InputSystem;
using CMGTSA.Core;
using CMGTSA.Game;

namespace CMGTSA.UI
{
    /// <summary>
    /// Game-over overlay panel. Hidden by default. Activates on <see cref="GameOverEvent"/>.
    /// While visible, polls Keyboard.R and tells <see cref="GameManager"/> to reload the scene.
    /// </summary>
    public class GameOverUI : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private GameManager gameManager;

        private bool visible;

        private void Awake()
        {
            if (panelRoot != null) panelRoot.SetActive(false);
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

        private void OnGameOver(GameOverEvent _)
        {
            visible = true;
            if (panelRoot != null) panelRoot.SetActive(true);
        }
    }
}
