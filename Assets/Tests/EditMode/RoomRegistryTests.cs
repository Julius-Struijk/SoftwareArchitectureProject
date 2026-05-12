using NUnit.Framework;
using UnityEngine;
using CMGTSA.Core;
using CMGTSA.Spawner;

namespace CMGTSA.Tests.EditMode
{
    public class RoomRegistryTests
    {
        private GameObject registryGo;
        private RoomRegistry registry;
        private GameObject roomAGo;
        private GameObject roomBGo;
        private Room roomA;
        private Room roomB;

        [SetUp]
        public void SetUp()
        {
            EventBus<PlayerEnteredRoomEvent>.Clear();
            EventBus<PlayerExitedRoomEvent>.Clear();

            roomAGo = new GameObject("RoomA");
            roomA = roomAGo.AddComponent<Room>();
            roomBGo = new GameObject("RoomB");
            roomB = roomBGo.AddComponent<Room>();

            registryGo = new GameObject("RoomRegistry");
            registry = registryGo.AddComponent<RoomRegistry>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(registryGo);
            Object.DestroyImmediate(roomAGo);
            Object.DestroyImmediate(roomBGo);
            EventBus<PlayerEnteredRoomEvent>.Clear();
            EventBus<PlayerExitedRoomEvent>.Clear();
        }

        [Test]
        public void Awake_CollectsAllRoomsInScene()
        {
            CollectionAssert.Contains(registry.AllRooms, roomA);
            CollectionAssert.Contains(registry.AllRooms, roomB);
        }

        [Test]
        public void OnPlayerEnter_SetsCurrentRoom()
        {
            EventBus<PlayerEnteredRoomEvent>.Publish(new PlayerEnteredRoomEvent(roomA));
            Assert.AreSame(roomA, registry.CurrentRoom);

            EventBus<PlayerEnteredRoomEvent>.Publish(new PlayerEnteredRoomEvent(roomB));
            Assert.AreSame(roomB, registry.CurrentRoom);
        }

        [Test]
        public void OnPlayerExit_ClearsCurrentRoomIfMatches()
        {
            EventBus<PlayerEnteredRoomEvent>.Publish(new PlayerEnteredRoomEvent(roomA));
            EventBus<PlayerExitedRoomEvent>.Publish(new PlayerExitedRoomEvent(roomA));
            Assert.IsNull(registry.CurrentRoom);
        }

        [Test]
        public void OnPlayerExit_DoesNotClearIfDifferentRoom()
        {
            EventBus<PlayerEnteredRoomEvent>.Publish(new PlayerEnteredRoomEvent(roomA));
            EventBus<PlayerExitedRoomEvent>.Publish(new PlayerExitedRoomEvent(roomB));
            Assert.AreSame(roomA, registry.CurrentRoom);
        }
    }
}
