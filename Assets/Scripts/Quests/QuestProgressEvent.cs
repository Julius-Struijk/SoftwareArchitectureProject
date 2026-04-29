using CMGTSA.Core;

namespace CMGTSA.Quests
{
    /// <summary>
    /// Published by <see cref="QuestManager"/> whenever any goal runtime on an active quest
    /// reports a state change (count incremented). Carries the runtime <see cref="QuestProgress"/>
    /// reference so the UI can re-render without searching for the right card.
    /// </summary>
    public readonly struct QuestProgressEvent : IGameEvent
    {
        public readonly QuestData Quest;
        public readonly QuestProgress Progress;

        public QuestProgressEvent(QuestData quest, QuestProgress progress)
        {
            Quest = quest;
            Progress = progress;
        }
    }
}
