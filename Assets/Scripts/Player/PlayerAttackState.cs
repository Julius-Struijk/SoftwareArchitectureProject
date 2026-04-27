using UnityEngine;
using CMGTSA.Core;
using CMGTSA.FSM;

namespace CMGTSA.Player
{
    /// <summary>
    /// Player attack: zeroes velocity, publishes a single <see cref="PlayerAttackRequestedEvent"/>
    /// on Enter, and transitions back to Idle once <see cref="PlayerController.AttackInterval"/>
    /// has elapsed.
    /// </summary>
    public class PlayerAttackState : State
    {
        private readonly PlayerController owner;
        private float startTime;

        public PlayerAttackState(PlayerController pOwner) { owner = pOwner; }

        public override void Enter()
        {
            base.Enter();
            owner.Body.linearVelocity = Vector2.zero;
            startTime = Time.time;

            EventBus<PlayerAttackRequestedEvent>.Publish(new PlayerAttackRequestedEvent(
                owner.transform.position,
                owner.LastFacing,
                owner.AttackRange,
                owner.AttackDamage));
        }

        public bool Finished() => Time.time > startTime + owner.AttackInterval;
    }
}
