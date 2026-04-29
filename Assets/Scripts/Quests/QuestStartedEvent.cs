using CMGTSA.Core;

namespace CMGTSA.Quests
{
    /// <summary>
    /// Published by <see cref="QuestManager"/> the moment a quest becomes active (game start
    /// for starter quests, future: on quest-giver interaction). Subscribers: <c>QuestUIPresenter</c>
    /// (spawn a card), audio-cue subscriber in slice 7.
    /// </summary>
    public readonly struct QuestStartedEvent : IGameEvent
    {
        public readonly QuestData Quest;
        public readonly QuestProgress Progress;

        public QuestStartedEvent(QuestData quest, QuestProgress progress)
        {
            Quest = quest;
            Progress = progress;
        }
    }
}
