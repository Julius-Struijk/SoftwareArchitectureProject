using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using CMGTSA.Core;
using CMGTSA.Enemies;
using CMGTSA.Spawner;

namespace CMGTSA.Tests.EditMode
{
    public class EnemySpawnerCoreTests
    {
        private class StubSampler : INavMeshSampler
        {
            public bool AcceptAll = true;
            public System.Predicate<Vector3> Predicate;
            public readonly List<Vector3> Samples = new List<Vector3>();

            public bool TrySample(Vector3 source, float maxDistance, out Vector3 valid)
            {
                Samples.Add(source);
                bool accept = Predicate != null ? Predicate(source) : AcceptAll;
                valid = source;
                return accept;
            }
        }

        private readonly List<EnemySpawnedEvent> publishedSpawns = new List<EnemySpawnedEvent>();
        private System.Action<EnemySpawnedEvent> spawnRecorder;

        private readonly List<GameObject> spawnedObjects = new List<GameObject>();
        private readonly List<Room> trackedRooms = new List<Room>();
        private EnemyData data;

        [SetUp]
        public void SetUp()
        {
            EventBus<EnemySpawnedEvent>.Clear();
            publishedSpawns.Clear();
            spawnRecorder = e => publishedSpawns.Add(e);
            EventBus<EnemySpawnedEvent>.Subscribe(spawnRecorder);

            data = ScriptableObject.CreateInstance<EnemyData>();
        }

        [TearDown]
        public void TearDown()
        {
            EventBus<EnemySpawnedEvent>.Unsubscribe(spawnRecorder);
            EventBus<EnemySpawnedEvent>.Clear();

            foreach (var go in spawnedObjects) if (go != null) Object.DestroyImmediate(go);
            foreach (var r in trackedRooms) if (r != null) Object.DestroyImmediate(r.gameObject);
            spawnedObjects.Clear();
            trackedRooms.Clear();

            Object.DestroyImmediate(data);
        }

        private Room MakeRoom(int cap, float interval, Bounds bounds, params EnemyData[] pool)
        {
            var go = new GameObject("Room");
            var room = go.AddComponent<Room>();
            room.Configure(pool, cap, interval, bounds);
            trackedRooms.Add(room);
            return room;
        }

        private EnemySpawner.Core.EnemyFactory FactoryThatRecords()
        {
            return (d, pos, room) =>
            {
                var go = new GameObject($"SpawnedEnemy@{pos.x:F1},{pos.y:F1}");
                go.transform.position = pos;
                go.AddComponent<EnemyRoomTag>().Room = room;
                spawnedObjects.Add(go);
                return go;
            };
        }

        [Test]
        public void Tick_BelowCap_AndIntervalElapsed_SpawnsOneEnemyAndUpdatesCounters()
        {
            var sampler = new StubSampler();
            var room = MakeRoom(cap: 3, interval: 0f,
                bounds: new Bounds(Vector3.zero, new Vector3(4f, 4f, 0f)), pool: data);

            var core = new EnemySpawner.Core(
                now: () => 1f,
                random01: () => 0.5f,
                sampler: sampler,
                factory: FactoryThatRecords(),
                maxAttempts: 4,
                sampleDistance: 0.5f);

            core.Tick(new[] { room });

            Assert.AreEqual(1, publishedSpawns.Count, "Exactly one EnemySpawnedEvent expected.");
            Assert.AreEqual(1, room.AliveCount);
            Assert.AreEqual(1f, room.LastSpawnAt, 0.0001f);
            Assert.AreSame(room, publishedSpawns[0].Room);
            Assert.AreSame(data, publishedSpawns[0].Data);
            Assert.IsNotNull(publishedSpawns[0].Enemy);
        }

        [Test]
        public void Tick_AtCap_DoesNotSpawn()
        {
            var sampler = new StubSampler();
            var room = MakeRoom(cap: 2, interval: 0f,
                bounds: new Bounds(Vector3.zero, new Vector3(4f, 4f, 0f)), pool: data);
            room.IncrementAliveCount();
            room.IncrementAliveCount();

            var core = new EnemySpawner.Core(
                () => 5f, () => 0.5f, sampler, FactoryThatRecords(), 4, 0.5f);

            core.Tick(new[] { room });

            Assert.AreEqual(0, publishedSpawns.Count);
            Assert.AreEqual(2, room.AliveCount, "Cap must hold.");
            Assert.AreEqual(0f, room.LastSpawnAt, 0.0001f, "LastSpawnAt must NOT be stamped on skipped tick.");
        }

        [Test]
        public void Tick_IntervalNotElapsed_DoesNotSpawn()
        {
            var sampler = new StubSampler();
            var room = MakeRoom(cap: 3, interval: 5f,
                bounds: new Bounds(Vector3.zero, new Vector3(4f, 4f, 0f)), pool: data);
            room.StampLastSpawnAt(1f);

            var core = new EnemySpawner.Core(
                () => 2f, () => 0.5f, sampler, FactoryThatRecords(), 4, 0.5f);

            core.Tick(new[] { room });

            Assert.AreEqual(0, publishedSpawns.Count);
            Assert.AreEqual(0, room.AliveCount);
        }

        [Test]
        public void Tick_AllSampleAttemptsFail_NoSpawnNoException()
        {
            var sampler = new StubSampler { AcceptAll = false };
            var room = MakeRoom(cap: 3, interval: 0f,
                bounds: new Bounds(Vector3.zero, new Vector3(4f, 4f, 0f)), pool: data);

            var core = new EnemySpawner.Core(
                () => 1f, () => 0.5f, sampler, FactoryThatRecords(), maxAttempts: 6, sampleDistance: 0.5f);

            Assert.DoesNotThrow(() => core.Tick(new[] { room }));
            Assert.AreEqual(0, publishedSpawns.Count);
            Assert.AreEqual(0, room.AliveCount);
            Assert.AreEqual(6, sampler.Samples.Count, "Spawner must use full maxAttempts budget before giving up.");
        }

        [Test]
        public void Tick_NullPoolOrEmptyPool_NoSpawn()
        {
            var sampler = new StubSampler();
            var roomEmpty = MakeRoom(cap: 3, interval: 0f,
                bounds: new Bounds(Vector3.zero, new Vector3(4f, 4f, 0f)), pool: new EnemyData[0]);
            var roomNull = MakeRoom(cap: 3, interval: 0f,
                bounds: new Bounds(Vector3.zero, new Vector3(4f, 4f, 0f)), pool: null);

            var core = new EnemySpawner.Core(
                () => 1f, () => 0.5f, sampler, FactoryThatRecords(), 4, 0.5f);

            core.Tick(new[] { roomEmpty, roomNull });

            Assert.AreEqual(0, publishedSpawns.Count);
        }

        [Test]
        public void OnEnemyDied_TaggedEnemy_DecrementsThatRoomsCounter()
        {
            var sampler = new StubSampler();
            var room = MakeRoom(cap: 3, interval: 0f,
                bounds: new Bounds(Vector3.zero, new Vector3(4f, 4f, 0f)), pool: data);

            var core = new EnemySpawner.Core(
                () => 1f, () => 0.5f, sampler, FactoryThatRecords(), 4, 0.5f);

            core.Tick(new[] { room });
            Assert.AreEqual(1, room.AliveCount);

            var spawnedGo = publishedSpawns[0].Enemy;
            core.OnEnemyDied(new EnemyDiedEvent(0, 0, Vector3.zero, data, spawnedGo));

            Assert.AreEqual(0, room.AliveCount);
        }

        [Test]
        public void OnEnemyDied_UntaggedEnemy_DoesNothing()
        {
            var sampler = new StubSampler();
            var room = MakeRoom(cap: 3, interval: 0f,
                bounds: new Bounds(Vector3.zero, new Vector3(4f, 4f, 0f)), pool: data);
            room.IncrementAliveCount();

            var core = new EnemySpawner.Core(
                () => 1f, () => 0.5f, sampler, FactoryThatRecords(), 4, 0.5f);

            var handPlaced = new GameObject("HandPlacedEnemy");
            core.OnEnemyDied(new EnemyDiedEvent(0, 0, Vector3.zero, data, handPlaced));

            Assert.AreEqual(1, room.AliveCount, "Hand-placed (untagged) enemies must not affect spawner counters.");
            Object.DestroyImmediate(handPlaced);
        }
    }
}
