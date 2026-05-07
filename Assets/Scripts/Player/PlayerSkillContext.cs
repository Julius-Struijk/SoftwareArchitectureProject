using UnityEngine;
using CMGTSA.Battle;
using CMGTSA.Core;
using CMGTSA.Skills;

namespace CMGTSA.Player
{
    public class PlayerSkillContext : ISkillContext
    {
        private readonly PlayerController player;
        private readonly IPhysicsLineCaster physics;

        public PlayerSkillContext(PlayerController player)
        {
            this.player = player;
            this.physics = new UnityPhysicsLineCaster();
        }

        public Vector3 PlayerPosition => player != null ? player.transform.position : Vector3.zero;
        public Vector2 PlayerFacing   => player != null ? player.LastFacing : Vector2.right;

        public IPhysicsLineCaster Physics => physics;

        public void ApplySpeedMultiplier(float multiplier)
        {
            if (player == null) return;
            player.ApplySpeedMultiplier(multiplier);
        }

        public void ApplyAreaDamage(Vector3 origin, float radius, DamageData damage)
        {
            EventBus<PlayerAttackRequestedEvent>.Publish(new PlayerAttackRequestedEvent(
                origin, Vector2.zero, radius, damage));
        }

        public void TeleportPlayer(Vector3 destination)
        {
            if (player == null) return;
            player.Teleport(destination);
        }
    }
}
