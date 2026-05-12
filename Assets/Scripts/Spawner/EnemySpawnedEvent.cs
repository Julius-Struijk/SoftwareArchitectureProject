using UnityEngine;
using CMGTSA.Core;
using CMGTSA.Enemies;

namespace CMGTSA.Spawner
{
    public readonly struct EnemySpawnedEvent : IGameEvent
    {
        public readonly GameObject Enemy;
        public readonly Room Room;
        public readonly EnemyData Data;

        public EnemySpawnedEvent(GameObject enemy, Room room, EnemyData data)
        {
            Enemy = enemy;
            Room = room;
            Data = data;
        }
    }
}
