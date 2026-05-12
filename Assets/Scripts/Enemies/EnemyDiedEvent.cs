using UnityEngine;
using CMGTSA.Core;

namespace CMGTSA.Enemies
{
    /// <summary>
    /// Published by <see cref="EnemyController"/> when the enemy's HP first hits zero,
    /// and by <c>BossDeadState</c> when the boss dies. Subscribers:
    /// <c>PlayerController</c> (XP/Money grant), <c>QuestManager</c>, <c>LootDropper</c>,
    /// <c>SFXController</c>, <c>DeathVFXSpawner</c>, and <c>EnemySpawner</c>
    /// — which reads <see cref="EnemyObject"/>'s <c>EnemyRoomTag</c> to decrement room counters.
    /// </summary>
    public readonly struct EnemyDiedEvent : IGameEvent
    {
        public readonly int XP;
        public readonly int Money;
        public readonly Vector3 Position;
        public readonly EnemyData Source;
        public readonly GameObject EnemyObject;

        public EnemyDiedEvent(int xp, int money, Vector3 position, EnemyData source, GameObject enemyObject)
        {
            XP = xp;
            Money = money;
            Position = position;
            Source = source;
            EnemyObject = enemyObject;
        }
    }
}
