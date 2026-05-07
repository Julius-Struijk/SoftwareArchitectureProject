using CMGTSA.Core;

namespace CMGTSA.Game
{
    public readonly struct DebugCheatToggledEvent : IGameEvent
    {
        public DebugCheat Cheat { get; }
        public int Magnitude { get; }
        public DebugCheatToggledEvent(DebugCheat cheat, int magnitude = 0)
        {
            Cheat = cheat;
            Magnitude = magnitude;
        }
    }
}
