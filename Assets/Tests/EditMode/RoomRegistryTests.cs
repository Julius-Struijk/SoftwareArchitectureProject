using NUnit.Framework;
using UnityEngine;
using CMGTSA.Core;
using CMGTSA.Spawner;

namespace CMGTSA.Tests.EditMode
{
    /// <summary>
    /// Tests for <see cref="RoomRegistry"/>. All tests call the public test seams
    /// (<see cref="RoomRegistry.InjectRooms"/> and <see cref="RoomRegistry.StartListening"/>)
    /// explicitly because Unity's EditMode runner does not fire <c>Awake</c> or <c>OnEnable</c>
    /// on <c>AddComponent</c> outside of play mode.
    /// </summary>
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

            // Lifecycle methods don't fire in EditMode — call seams explicitly.
            registry.InjectRooms(new[] { roomA, roomB });
            registry.StartListening();
        }

        [TearDown]
        public void TearDown()
        {
            registry.StopListening();
            Object.DestroyImmediate(registryGo);
            Object.DestroyImmediate(roomAGo);
            Object.DestroyImmediate(roomBGo);
            EventBus<PlayerEnteredRoomEvent>.Clear();
            EventBus<PlayerExitedRoomEvent>.Clear();
        }

        [Test]
        public void InjectRooms_PopulatesAllRooms()
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
