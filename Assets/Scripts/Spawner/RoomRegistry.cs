using System.Collections.Generic;
using UnityEngine;
using CMGTSA.Core;

namespace CMGTSA.Spawner
{
    /// <summary>
    /// Scene-singleton registry of every <see cref="Room"/>. Collects rooms on <c>Awake</c>
    /// and tracks the player's current room via <see cref="PlayerEnteredRoomEvent"/> /
    /// <see cref="PlayerExitedRoomEvent"/>. <see cref="EnemySpawner"/> reads
    /// <see cref="AllRooms"/> each tick to iterate spawn candidates.
    /// </summary>
    public class RoomRegistry : MonoBehaviour
    {
        private readonly List<Room> rooms = new List<Room>();

        public IReadOnlyList<Room> AllRooms => rooms;
        public Room CurrentRoom { get; private set; }

        private void Awake()
        {
            // Room.Awake populates Bounds from BoxCollider2D. Both Awake calls run in the same
            // phase, so ordering is not guaranteed. RoomRegistry only stores references here;
            // callers that need valid Bounds should access them after Start (e.g. EnemySpawner.Update).
            CollectRooms();
        }

        private void OnEnable() => StartListening();
        private void OnDisable() => StopListening();

        /// <summary>
        /// Runtime room collection: scans the scene for all <see cref="Room"/> instances.
        /// Called from <c>Awake</c> in production; also public so EditMode tests can trigger
        /// it explicitly (EditMode does not fire <c>Awake</c> on <c>AddComponent</c>).
        /// </summary>
        public void CollectRooms()
        {
            rooms.Clear();
            var found = FindObjectsByType<Room>(FindObjectsSortMode.None);
            for (int i = 0; i < found.Length; i++)
            {
                rooms.Add(found[i]);
            }
        }

        /// <summary>
        /// Test seam: inject a known set of rooms directly, bypassing <see cref="CollectRooms"/>.
        /// In EditMode tests <c>FindObjectsByType</c> does not see components added via
        /// <c>AddComponent</c> in the same test setup, so tests call this instead.
        /// </summary>
        public void InjectRooms(Room[] injectedRooms)
        {
            rooms.Clear();
            if (injectedRooms == null) return;
            for (int i = 0; i < injectedRooms.Length; i++)
                rooms.Add(injectedRooms[i]);
        }

        /// <summary>
        /// Subscribes to enter/exit events. Called from <c>OnEnable</c> in production.
        /// Public so EditMode tests can call it explicitly when <c>OnEnable</c> does not fire.
        /// Unsubscribes first so the method is safe to call multiple times without
        /// double-registering (idempotent).
        /// </summary>
        public void StartListening()
        {
            EventBus<PlayerEnteredRoomEvent>.Unsubscribe(OnPlayerEntered);
            EventBus<PlayerEnteredRoomEvent>.Subscribe(OnPlayerEntered);
            EventBus<PlayerExitedRoomEvent>.Unsubscribe(OnPlayerExited);
            EventBus<PlayerExitedRoomEvent>.Subscribe(OnPlayerExited);
        }

        /// <summary>Unsubscribes from enter/exit events. Called from <c>OnDisable</c>.</summary>
        public void StopListening()
        {
            EventBus<PlayerEnteredRoomEvent>.Unsubscribe(OnPlayerEntered);
            EventBus<PlayerExitedRoomEvent>.Unsubscribe(OnPlayerExited);
        }

        private void OnPlayerEntered(PlayerEnteredRoomEvent evt)
        {
            CurrentRoom = evt.Room;
        }

        private void OnPlayerExited(PlayerExitedRoomEvent evt)
        {
            if (CurrentRoom == evt.Room) CurrentRoom = null;
        }
    }
}
