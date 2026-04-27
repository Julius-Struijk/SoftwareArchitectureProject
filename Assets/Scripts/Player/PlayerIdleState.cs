using UnityEngine;
using CMGTSA.FSM;

namespace CMGTSA.Player
{
    /// <summary>
    /// Player idle: zero velocity, wait for input. Transitions to Move on movement input
    /// and to Attack on attack input.
    /// </summary>
    public class PlayerIdleState : State
    {
        private readonly PlayerController owner;

        public PlayerIdleState(PlayerController pOwner) { owner = pOwner; }

        public override void Enter()
        {
            base.Enter();
            owner.Body.linearVelocity = Vector2.zero;
        }

        public bool WantsToMove() => owner.Input.MoveInput.sqrMagnitude > 0.0001f;
        public bool WantsToAttack() => owner.Input.ConsumeAttackQueued();
    }
}
