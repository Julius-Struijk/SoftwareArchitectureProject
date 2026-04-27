using CMGTSA.Core;

namespace CMGTSA.Game
{
    /// <summary>
    /// Published by <see cref="GameManager"/> in <c>Start</c> when a fresh scene load is detected.
    /// Subscribers: every system that needs to reset transient state (slice 4+).
    /// </summary>
    public readonly struct GameRestartedEvent : IGameEvent { }
}
