using CMGTSA.Core;

namespace CMGTSA.Player
{
    /// <summary>
    /// Published by <see cref="PlayerStatsModel"/> once per crossed level threshold
    /// (inside the <c>GainXP</c> loop). Carries the new level and the *new* XP
    /// threshold so the level-up VFX, SFX, SkillManager, and HUD level-text
    /// presenter all stay stateless w.r.t. stats.
    /// Subscribers: <see cref="CMGTSA.UI.HUDLevelTextPresenter"/>,
    /// <see cref="CMGTSA.Skills.SkillManager"/>, level-up VFX, level-up SFX,
    /// <see cref="PlayerController"/> (applies <see cref="LevelUpRewardsTable"/>).
    /// </summary>
    public readonly struct PlayerLeveledUpEvent : IGameEvent
    {
        public readonly int NewLevel;
        public readonly int XPForNextLevel;

        public PlayerLeveledUpEvent(int newLevel, int xpForNextLevel)
        {
            NewLevel = newLevel;
            XPForNextLevel = xpForNextLevel;
        }
    }
}
