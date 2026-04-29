using UnityEngine;
using UnityEngine.InputSystem;

namespace CMGTSA.UI
{
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
    }
}
