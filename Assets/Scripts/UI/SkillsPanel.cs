using UnityEngine;
using UnityEngine.InputSystem;

namespace CMGTSA.UI
{
    /// <summary>
    /// Toggles the skills panel root on K. Polls Keyboard directly to match
    /// <see cref="InventoryPanel"/> / <see cref="GameOverUI"/>'s convention.
    ///
    /// IMPORTANT: this component MUST live on a GameObject that stays active for
    /// the whole game session. <see cref="panelRoot"/> must point at a *different*
    /// GameObject (typically a child of this one's parent — never this GameObject
    /// or a descendant). Otherwise <see cref="GameObject.SetActive(bool)"/> on
    /// <see cref="panelRoot"/> also deactivates this MonoBehaviour, freezing
    /// <see cref="Update"/> and making the panel impossible to re-show.
    /// <see cref="OnValidate"/> warns in the Editor if the wiring is wrong.
    /// </summary>
    public class SkillsPanel : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private bool startVisible = true;

        private void Awake()
        {
            if (panelRoot != null) panelRoot.SetActive(startVisible);
        }

        private void Update()
        {
            if (Keyboard.current == null) return;
            if (Keyboard.current.kKey.wasPressedThisFrame)
            {
                if (panelRoot != null) panelRoot.SetActive(!panelRoot.activeSelf);
            }
        }

        private void OnValidate()
        {
            if (panelRoot == null) return;
            // Warn when panelRoot.SetActive(false) would also deactivate this component —
            // i.e. this object IS panelRoot, or is nested inside panelRoot's subtree.
            if (panelRoot == gameObject || transform.IsChildOf(panelRoot.transform))
            {
                Debug.LogWarning(
                    "SkillsPanel: this GameObject must NOT be panelRoot or a descendant of it. " +
                    "Put SkillsPanel on a stable parent and point panelRoot at the panel child.",
                    this);
            }
        }
    }
}
