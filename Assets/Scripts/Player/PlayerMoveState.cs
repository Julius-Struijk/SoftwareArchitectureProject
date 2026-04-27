using UnityEngine;
using CMGTSA.FSM;

namespace CMGTSA.Player
{
    /// <summary>
    /// Player move: applies the input vector scaled by <see cref="PlayerController.MoveSpeed"/>
    /// to the Rigidbody2D linear velocity. Transitions back to Idle when input zeroes,
    /// to Attack on attack input.
    /// </summary>
    public class PlayerMoveState : State
    {
        private readonly PlayerController owner;

        public PlayerMoveState(PlayerController pOwner) { owner = pOwner; }

        public override void Step()
        {
            base.Step();
            owner.Body.linearVelocity = owner.Input.MoveInput * owner.MoveSpeed;
        }

        public bool IsStill() => owner.Input.MoveInput.sqrMagnitude < 0.0001f;
        public bool WantsToAttack() => owner.Input.ConsumeAttackQueued();
    }
}
