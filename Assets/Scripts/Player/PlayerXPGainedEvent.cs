using CMGTSA.Core;

namespace CMGTSA.Player
{
    /// <summary>
    /// Published by <see cref="PlayerStatsModel"/> after every <c>GainXP</c> call,
    /// once the level-up loop has settled. Carries the post-settlement total XP,
    /// the amount just gained, and the *current* threshold so HUD presenters can
    /// render <c>xp / xpForNextLevel</c> without needing a stats reference.
    /// Subscribers: HUD XP bar (<see cref="CMGTSA.UI.HUDXPBarPresenter"/>),
    /// HUD XP number (<see cref="CMGTSA.UI.HUDXPNumberPresenter"/>), SkillManager.
    /// </summary>
    public readonly struct PlayerXPGainedEvent : IGameEvent
    {
        public readonly int TotalXP;
        public readonly int Gained;
        public readonly int XPForNextLevel;

        public PlayerXPGainedEvent(int totalXP, int gained, int xpForNextLevel)
        {
            TotalXP = totalXP;
            Gained = gained;
            XPForNextLevel = xpForNextLevel;
        }
    }
}
