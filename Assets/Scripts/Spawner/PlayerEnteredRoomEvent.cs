using CMGTSA.Core;

namespace CMGTSA.Spawner
{
    public readonly struct PlayerEnteredRoomEvent : IGameEvent
    {
        public readonly Room Room;
        public PlayerEnteredRoomEvent(Room room) { Room = room; }
    }
}
