using System;
using System.Collections.Generic;
using UnityEngine;
using CMGTSA.Core;
using CMGTSA.Enemies;

namespace CMGTSA.Spawner
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private int maxAttempts = 8;
        [SerializeField] private float sampleDistance = 1f;

        private RoomRegistry registry;
        private Core core;

        private void Awake()
        {
            registry = FindAnyObjectByType<RoomRegistry>();
            if (registry == null)
            {
                Debug.LogWarning("EnemySpawner could not find a RoomRegistry in the scene. " +
                                 "No spawns will occur.", this);
            }

            core = new Core(
                now: () => Time.time,
                random01: () => UnityEngine.Random.value,
                sampler: new UnityNavMeshSampler(),
                factory: SpawnEnemyForRoom,
                maxAttempts: maxAttempts,
                sampleDistance: sampleDistance);
        }

        private void OnEnable()
        {
            EventBus<EnemyDiedEvent>.Subscribe(OnEnemyDied);
        }

        private void OnDisable()
        {
            EventBus<EnemyDiedEvent>.Unsubscribe(OnEnemyDied);
        }

        private void OnEnemyDied(EnemyDiedEvent evt)
        {
            core?.OnEnemyDied(evt);
        }

        private void Update()
        {
            if (registry == null || core == null) return;
            core.Tick(registry.AllRooms);
        }

        private GameObject SpawnEnemyForRoom(EnemyData data, Vector3 position, Room room)
        {
            if (data == null) return null;
            GameObject go = data.CreateEnemy(position);
            if (go == null) return null;
            EnemyRoomTag tag = go.AddComponent<EnemyRoomTag>();
            tag.Room = room;
            return go;
        }

        public class Core
        {
            public delegate GameObject EnemyFactory(EnemyData data, Vector3 position, Room room);

            private readonly Func<float> now;
            private readonly Func<float> random01;
            private readonly INavMeshSampler sampler;
            private readonly EnemyFactory factory;
            private readonly int maxAttempts;
            private readonly float sampleDistance;

            public Core(Func<float> now, Func<float> random01,
                        INavMeshSampler sampler, EnemyFactory factory,
                        int maxAttempts, float sampleDistance)
            {
                this.now = now;
                this.random01 = random01;
                this.sampler = sampler;
                this.factory = factory;
                this.maxAttempts = Mathf.Max(1, maxAttempts);
                this.sampleDistance = Mathf.Max(0.01f, sampleDistance);
            }

            public void Tick(IReadOnlyList<Room> rooms)
            {
                if (rooms == null) return;
                float t = now();

                for (int i = 0; i < rooms.Count; i++)
                {
                    Room room = rooms[i];
                    if (room == null) continue;
                    if (t - room.LastSpawnAt < room.RespawnInterval) continue;
                    if (room.AliveCount >= room.SimultaneousCap) continue;

                    var pool = room.EnemyPool;
                    if (pool == null || pool.Count == 0) continue;

                    EnemyData data = PickRandom(pool);
                    if (data == null) continue;

                    for (int attempt = 0; attempt < maxAttempts; attempt++)
                    {
                        Vector3 candidate = RandomPointInside(room.Bounds);
                        if (sampler.TrySample(candidate, sampleDistance, out Vector3 valid))
                        {
                            GameObject spawned = factory(data, valid, room);
                            if (spawned != null)
                            {
                                EventBus<EnemySpawnedEvent>.Publish(
                                    new EnemySpawnedEvent(spawned, room, data));
                                room.IncrementAliveCount();
                                room.StampLastSpawnAt(t);
                            }
                            break;
                        }
                    }
                }
            }

            public void OnEnemyDied(EnemyDiedEvent evt)
            {
                if (evt.EnemyObject == null) return;
                EnemyRoomTag tag = evt.EnemyObject.GetComponent<EnemyRoomTag>();
                if (tag == null || tag.Room == null) return;
                tag.Room.DecrementAliveCount();
            }

            private EnemyData PickRandom(IReadOnlyList<EnemyData> pool)
            {
                int idx = Mathf.Clamp((int)(random01() * pool.Count), 0, pool.Count - 1);
                return pool[idx];
            }

            private Vector3 RandomPointInside(Bounds b)
            {
                return new Vector3(
                    b.min.x + random01() * b.size.x,
                    b.min.y + random01() * b.size.y,
                    b.center.z);
            }
        }
    }
}
