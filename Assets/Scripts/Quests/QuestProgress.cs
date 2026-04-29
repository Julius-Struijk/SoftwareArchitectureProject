namespace CMGTSA.Quests
{
    /// <summary>
    /// Runtime container for one assigned quest. Created by <see cref="QuestManager"/> from
    /// a <see cref="QuestData"/> at start time. Holds the array of <see cref="IQuestGoalRuntime"/>
    /// instances (one per goal on the quest), in the order they appear on the SO. Exposes
    /// <see cref="IsComplete"/> as the AND of every goal — flips to <c>true</c> the moment
    /// the last goal finishes.
    /// </summary>
    public class QuestProgress
    {
        public QuestData Data { get; }
        public IQuestGoalRuntime[] Goals { get; }
        public bool CompletionPublished { get; set; }

        public QuestProgress(QuestData data, IQuestGoalRuntime[] goals)
        {
            Data = data;
            Goals = goals;
        }

        public bool IsComplete
        {
            get
            {
                if (Goals == null || Goals.Length == 0) return false;
                for (int i = 0; i < Goals.Length; i++)
                {
                    if (Goals[i] == null || !Goals[i].IsComplete) return false;
                }
                return true;
            }
        }
    }
}
