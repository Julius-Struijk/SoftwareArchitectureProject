using CMGTSA.Core;

namespace CMGTSA.Player
{
    /// <summary>
    /// Published by <see cref="PlayerStatsModel"/> whenever the player gains XP.
    /// Subscribers: HUD XP bar (slice 5), SkillManager (slice 5).
    /// </summary>
    public readonly struct PlayerXPGainedEvent : IGameEvent
    {
        public readonly int TotalXP;
        public readonly int Gained;

        public PlayerXPGainedEvent(int totalXP, int gained)
        {
            TotalXP = totalXP;
            Gained = gained;
        }
    }
}
