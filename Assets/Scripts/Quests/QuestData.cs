using UnityEngine;

namespace CMGTSA.Quests
{
    /// <summary>
    /// ScriptableObject Factory: produces a fresh <see cref="QuestProgress"/> per assignment
    /// from designer-tuned defaults. The <see cref="goals"/> array is polymorphic — designers
    /// drop any mix of <see cref="KillEnemyGoal"/>, <see cref="FetchItemGoal"/>, or future
    /// goal subclasses into the array, and the manager tracks them all the same way.
    /// </summary>
    [CreateAssetMenu(fileName = "QuestData", menuName = "Scriptable Objects/Quests/QuestData")]
    public class QuestData : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Player-facing quest title shown on the HUD card.")]
        public string displayName;

        [TextArea]
        [Tooltip("Optional flavour description shown under the title in tooltips.")]
        public string description;

        [Header("Goals")]
        [Tooltip("Polymorphic SO list — drop any concrete SOQuestGoal asset here.")]
        public SOQuestGoal[] goals;

        [Header("Reward")]
        [Tooltip("XP granted to the player on completion (carried by QuestCompletedEvent).")]
        [Min(0)] public int xpReward = 5;

        public QuestProgress CreateProgress()
        {
            var runtimes = goals != null ? new IQuestGoalRuntime[goals.Length] : System.Array.Empty<IQuestGoalRuntime>();
            for (int i = 0; goals != null && i < goals.Length; i++)
            {
                runtimes[i] = goals[i] != null ? goals[i].CreateRuntime() : null;
            }
            return new QuestProgress(this, runtimes);
        }
    }
}
