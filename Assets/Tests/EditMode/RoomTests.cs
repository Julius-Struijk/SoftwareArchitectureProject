using NUnit.Framework;
using UnityEngine;
using CMGTSA.Enemies;
using CMGTSA.Spawner;

namespace CMGTSA.Tests.EditMode
{
    public class RoomTests
    {
        private GameObject roomGo;
        private Room room;

        [SetUp]
        public void SetUp()
        {
            roomGo = new GameObject("TestRoom");
            room = roomGo.AddComponent<Room>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(roomGo);
        }

        [Test]
        public void Defaults_AliveZero_LastSpawnZero()
        {
            Assert.AreEqual(0, room.AliveCount);
            Assert.AreEqual(0f, room.LastSpawnAt, 0.0001f);
        }

        [Test]
        public void Configure_SetsPoolCapIntervalAndBounds()
        {
            var data = ScriptableObject.CreateInstance<EnemyData>();
            var bounds = new Bounds(new Vector3(1f, 2f, 0f), new Vector3(4f, 4f, 0f));

            room.Configure(new[] { data }, 5, 2.5f, bounds);

            Assert.AreEqual(1, room.EnemyPool.Count);
            Assert.AreSame(data, room.EnemyPool[0]);
            Assert.AreEqual(5, room.SimultaneousCap);
            Assert.AreEqual(2.5f, room.RespawnInterval, 0.0001f);
            Assert.AreEqual(bounds, room.Bounds);

            Object.DestroyImmediate(data);
        }

        [Test]
        public void IncrementAliveCount_IncrementsByOne()
        {
            room.IncrementAliveCount();
            room.IncrementAliveCount();
            Assert.AreEqual(2, room.AliveCount);
        }

        [Test]
        public void DecrementAliveCount_ClampsAtZero()
        {
            room.DecrementAliveCount();
            Assert.AreEqual(0, room.AliveCount);

            room.IncrementAliveCount();
            room.DecrementAliveCount();
            room.DecrementAliveCount();
            Assert.AreEqual(0, room.AliveCount);
        }

        [Test]
        public void StampLastSpawnAt_RecordsValue()
        {
            room.StampLastSpawnAt(7.25f);
            Assert.AreEqual(7.25f, room.LastSpawnAt, 0.0001f);
        }
    }
}
