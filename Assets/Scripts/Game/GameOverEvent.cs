using CMGTSA.Core;

namespace CMGTSA.Game
{
    /// <summary>
    /// Published by <see cref="GameManager"/> after a <c>PlayerDiedEvent</c>.
    /// Subscribers: <c>GameOverUI</c>, AudioManager (slice 7), HUDController (hide HUD).
    /// </summary>
    public readonly struct GameOverEvent : IGameEvent { }
}
