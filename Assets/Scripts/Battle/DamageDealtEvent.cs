using UnityEngine;
using CMGTSA.Core;

namespace CMGTSA.Battle
{
    /// <summary>
    /// Published by <see cref="DamageResolver"/> after a successful hit is resolved.
    /// Subscribers: target actor (for hurt-state transition / death VFX),
    /// hit-VFX, screen-shake, hit-SFX, damage-number spawner (slice 7).
    /// </summary>
    public readonly struct DamageDealtEvent : IGameEvent
    {
        public readonly Vector3 Position;
        public readonly int Amount;
        public readonly DamageData Damage;
        public readonly Transform Target;
        public readonly bool Killed;

        public DamageDealtEvent(Vector3 position, int amount, DamageData damage, Transform target, bool killed)
        {
            Position = position;
            Amount = amount;
            Damage = damage;
            Target = target;
            Killed = killed;
        }
    }
}
