using UnityEngine;
using UnityEngine.InputSystem;

namespace CMGTSA.Game
{
    /// <summary>
    /// The purpose of this class is to ensure only the default "Player" action map
    /// is activated, it does this by disabling other action maps aside from the default
    /// one after a very short delay
    /// </summary>
    [RequireComponent(typeof(PlayerInput))]
    public class InputManager : MonoBehaviour
    {
        private PlayerInput playerInput;

        private void Awake()
        {
            Invoke("DisableNonDefaultActionMaps", 0.05f);
        }

        private void DisableNonDefaultActionMaps()
        {
            playerInput = GetComponent<PlayerInput>();
            playerInput.SwitchCurrentActionMap("Player");
        }
    }
}
