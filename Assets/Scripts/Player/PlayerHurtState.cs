using UnityEngine;
using CMGTSA.FSM;

namespace CMGTSA.Player
{
    /// <summary>
    /// Player hurt: brief stun. Zeroes velocity on Enter, transitions back to Idle
    /// when <see cref="PlayerController.HurtDuration"/> has elapsed.
    /// </summary>
    public class PlayerHurtState : State
    {
        private readonly PlayerController owner;
        private float startTime;

        public PlayerHurtState(PlayerController pOwner) { owner = pOwner; }

        public override void Enter()
        {
            base.Enter();
            owner.Body.linearVelocity = Vector2.zero;
            startTime = Time.time;
        }

        public bool StunEnded() => Time.time > startTime + owner.HurtDuration;
    }
}
