using CMGTSA.Core;

namespace CMGTSA.Player
{
    /// <summary>
    /// Published by <see cref="PlayerStatsModel"/> when current HP first reaches zero.
    /// Subscribers: GameManager, GameOverUI.
    /// </summary>
    public readonly struct PlayerDiedEvent : IGameEvent { }
}
