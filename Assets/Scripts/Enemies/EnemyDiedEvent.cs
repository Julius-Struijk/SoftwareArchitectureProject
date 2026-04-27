using UnityEngine;
using CMGTSA.Core;

namespace CMGTSA.Enemies
{
    /// <summary>
    /// Published by <see cref="EnemyController"/> when the enemy's HP first hits zero.
    /// Subscribers: <c>PlayerController</c> (XP/Money grant), QuestManager (slice 4),
    /// loot dropper (slice 3).
    /// </summary>
    public readonly struct EnemyDiedEvent : IGameEvent
    {
        public readonly int XP;
        public readonly int Money;
        public readonly Vector3 Position;
        public readonly EnemyData Source;

        public EnemyDiedEvent(int xp, int money, Vector3 position, EnemyData source)
        {
            XP = xp;
            Money = money;
            Position = position;
            Source = source;
        }
    }
}
