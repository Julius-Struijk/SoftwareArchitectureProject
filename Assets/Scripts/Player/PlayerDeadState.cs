using UnityEngine;
using CMGTSA.FSM;

namespace CMGTSA.Player
{
    /// <summary>
    /// Player dead: terminal state. Zeroes velocity. <c>PlayerStatsModel</c> already
    /// publishes <c>PlayerDiedEvent</c> at HP 0; <see cref="GameManager"/> handles the
    /// scene reload from there. No transitions out.
    /// </summary>
    public class PlayerDeadState : State
    {
        private readonly PlayerController owner;

        public PlayerDeadState(PlayerController pOwner) { owner = pOwner; }

        public override void Enter()
        {
            base.Enter();
            owner.Body.linearVelocity = Vector2.zero;
        }
    }
}
