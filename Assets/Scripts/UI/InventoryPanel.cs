using UnityEngine;
using UnityEngine.InputSystem;

namespace CMGTSA.UI
{
    /// <summary>
    /// Toggles the inventory panel root on Tab. Polls Keyboard directly to match
    /// <c>GameOverUI</c>'s convention; UnityEvents/InputAction wiring is intentionally
    /// avoided so a reviewer reads the show/hide rule in one place.
    /// </summary>
    public class InventoryPanel : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private bool startVisible = false;

        private void Awake()
        {
            if (panelRoot != null) panelRoot.SetActive(startVisible);
        }

        private void Update()
        {
            if (Keyboard.current == null) return;
            if (Keyboard.current.tabKey.wasPressedThisFrame)
            {
                if (panelRoot != null) panelRoot.SetActive(!panelRoot.activeSelf);
            }
        }
    }
}
