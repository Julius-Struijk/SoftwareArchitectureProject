using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CMGTSA.Player
{
    /// <summary>
    /// Pure input adapter. Stores the latest <see cref="MoveInput"/> vector and a
    /// one-shot attack flag readable via <see cref="ConsumeAttackQueued"/>. The FSM in
    /// <see cref="PlayerController"/> reads these and applies physics in MoveState.
    /// </summary>
    public class PlayerControl : MonoBehaviour
    {
        public static Action onInteract;

        public Vector2 MoveInput { get; private set; }

        private bool attackQueued;

        public bool ConsumeAttackQueued()
        {
            if (!attackQueued) return false;
            attackQueued = false;
            return true;
        }

        public void Move(InputAction.CallbackContext context)
        {
            MoveInput = context.ReadValue<Vector2>();
        }

        public void Attack(InputAction.CallbackContext context)
        {
            // Latch on Performed; cleared by ConsumeAttackQueued in the FSM transition predicate.
            if (context.performed) attackQueued = true;
        }

        public void Interact(InputAction.CallbackContext context)
        {
            if (context.performed) onInteract?.Invoke();
        }
    }
}
