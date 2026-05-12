using UnityEngine;
using CMGTSA.Core;

namespace CMGTSA.Spawner
{
    [RequireComponent(typeof(Room))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class RoomTrigger : MonoBehaviour
    {
        private Room room;

        private void Awake()
        {
            room = GetComponent<Room>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            EventBus<PlayerEnteredRoomEvent>.Publish(new PlayerEnteredRoomEvent(room));
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            EventBus<PlayerExitedRoomEvent>.Publish(new PlayerExitedRoomEvent(room));
        }
    }
}
