using UnityEngine;
using CMGTSA.Core;
using CMGTSA.Battle;

namespace CMGTSA.Player
{
    /// <summary>
    /// Published by <see cref="PlayerAttackState"/> when the player's swing connects.
    /// Subscribers: <see cref="DamageResolver"/> (resolves overlap + damage).
    /// </summary>
    public readonly struct PlayerAttackRequestedEvent : IGameEvent
    {
        public readonly Vector3 Origin;
        public readonly Vector2 Direction;
        public readonly float Range;
        public readonly DamageData Damage;

        public PlayerAttackRequestedEvent(Vector3 origin, Vector2 direction, float range, DamageData damage)
        {
            Origin = origin;
            Direction = direction;
            Range = range;
            Damage = damage;
        }
    }
}
