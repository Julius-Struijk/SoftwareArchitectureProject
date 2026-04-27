using CMGTSA.Core;

namespace CMGTSA.Player
{
    /// <summary>
    /// Published by <see cref="PlayerStatsModel"/> when the level threshold is crossed.
    /// Subscribers: HUD level number, SkillManager (slice 5), level-up VFX (slice 7).
    /// </summary>
    public readonly struct PlayerLeveledUpEvent : IGameEvent
    {
        public readonly int NewLevel;

        public PlayerLeveledUpEvent(int newLevel)
        {
            NewLevel = newLevel;
        }
    }
}
