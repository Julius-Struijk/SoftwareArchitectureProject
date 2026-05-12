using CMGTSA.Core;

namespace CMGTSA.Spawner
{
    public readonly struct PlayerExitedRoomEvent : IGameEvent
    {
        public readonly Room Room;
        public PlayerExitedRoomEvent(Room room) { Room = room; }
    }
}
