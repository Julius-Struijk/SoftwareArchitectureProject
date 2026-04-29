using CMGTSA.Core;

namespace CMGTSA.Quests
{
    /// <summary>
    /// Published by <see cref="QuestManager"/> the first time every goal on an active quest
    /// reports complete. Subscribers: <c>QuestUIPresenter</c> (flip card to "Complete!"),
    /// <c>PlayerController</c> (grant <see cref="XPReward"/> via <c>PlayerStatsModel.GainXP</c>).
    /// Carries the XP reward inline so the player system never reads <see cref="QuestData"/>
    /// directly — keeps the cross-system rule "no direct cross-system references".
    /// </summary>
    public readonly struct QuestCompletedEvent : IGameEvent
    {
        public readonly QuestData Quest;
        public readonly int XPReward;

        public QuestCompletedEvent(QuestData quest, int xpReward)
        {
            Quest = quest;
            XPReward = xpReward;
        }
    }
}
