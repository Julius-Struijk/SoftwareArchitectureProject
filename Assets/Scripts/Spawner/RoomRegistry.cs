using System.Collections.Generic;
using UnityEngine;
using CMGTSA.Core;

namespace CMGTSA.Spawner
{
    public class RoomRegistry : MonoBehaviour
    {
        private readonly List<Room> rooms = new List<Room>();

        public IReadOnlyList<Room> AllRooms => rooms;
        public Room CurrentRoom { get; private set; }

        private void Awake()
        {
            rooms.Clear();
            var found = FindObjectsByType<Room>(FindObjectsSortMode.None);
            for (int i = 0; i < found.Length; i++)
            {
                rooms.Add(found[i]);
            }
        }

        private void OnEnable()
        {
            EventBus<PlayerEnteredRoomEvent>.Subscribe(OnPlayerEntered);
            EventBus<PlayerExitedRoomEvent>.Subscribe(OnPlayerExited);
        }

        private void OnDisable()
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
