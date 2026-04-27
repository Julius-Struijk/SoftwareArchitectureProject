using UnityEngine;
using CMGTSA.Core;
using CMGTSA.Battle;

namespace CMGTSA.Enemies
{
    /// <summary>
    /// Published by <see cref="EnemyController"/> when its FSM enters the AttackState.
    /// Subscribers: <see cref="DamageResolver"/>.
    /// </summary>
    public readonly struct EnemyAttackRequestedEvent : IGameEvent
    {
        public readonly Vector3 Origin;
        public readonly float Range;
        public readonly DamageData Damage;
        public readonly Transform Source;

        public EnemyAttackRequestedEvent(Vector3 origin, float range, DamageData damage, Transform source)
        {
            Origin = origin;
            Range = range;
            Damage = damage;
            Source = source;
        }
    }
}
