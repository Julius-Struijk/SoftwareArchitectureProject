using UnityEngine;
using UnityEngine.InputSystem;
using CMGTSA.Boss;
using CMGTSA.Core;
using CMGTSA.Game;

namespace CMGTSA.UI
{
    public class VictoryUI : MonoBehaviour
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
            EventBus<BossEncounterEndedEvent>.Subscribe(OnEncounterEnded);
        }

        private void OnDisable()
        {
            EventBus<BossEncounterEndedEvent>.Unsubscribe(OnEncounterEnded);
        }

        private void Update()
        {
            if (!visible) return;
            if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
            {
                if (gameManager != null) gameManager.ReloadScene();
            }
        }

        private void OnEncounterEnded(BossEncounterEndedEvent evt)
        {
            if (!evt.Victory) return;
            SetPanelVisible(true);
        }

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
