using CMGTSA.Enemies;
using CMGTSA.Inventory;

namespace CMGTSA.Quests
{
    /// <summary>
    /// Runtime tracker for one goal of one assigned quest. Each subclass overrides only the
    /// event hooks it cares about; the base no-ops let <see cref="QuestManager"/> blindly
    /// dispatch every event to every active goal without type-checking.
    ///
    /// Hooks return <c>true</c> if state changed (so the manager knows to publish
    /// <see cref="QuestProgressEvent"/>). Default returns <c>false</c>.
    /// </summary>
    public abstract class IQuestGoalRuntime
    {
        public abstract int CurrentCount { get; }
        public abstract int RequiredCount { get; }
        public bool IsComplete => CurrentCount >= RequiredCount;

        /// <summary>Player-facing label including current/required counts.</summary>
        public abstract string Describe();

        public virtual bool OnEnemyDied(EnemyDiedEvent evt) => false;
        public virtual bool OnItemPickedUp(ItemPickedUpEvent evt) => false;
    }
}
