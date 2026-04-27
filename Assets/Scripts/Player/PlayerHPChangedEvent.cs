using CMGTSA.Core;

namespace CMGTSA.Player
{
    /// <summary>
    /// Published by <see cref="PlayerStatsModel"/> whenever the player's current HP changes.
    /// Subscribers: HUD HP-bar presenter, HUD HP-number presenter, GameManager (death gating).
    /// </summary>
    public readonly struct PlayerHPChangedEvent : IGameEvent
    {
        public readonly int CurrentHP;
        public readonly int MaxHP;
        public readonly int Delta;

        public PlayerHPChangedEvent(int currentHP, int maxHP, int delta)
        {
            CurrentHP = currentHP;
            MaxHP = maxHP;
            Delta = delta;
        }
    }
}
