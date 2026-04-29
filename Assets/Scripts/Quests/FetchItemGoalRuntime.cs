using CMGTSA.Inventory;

namespace CMGTSA.Quests
{
    /// <summary>
    /// Runtime tracker for a <see cref="FetchItemGoal"/>. Increments <see cref="CurrentCount"/>
    /// each time <see cref="ItemPickedUpEvent.Item"/> matches the goal's target item.
    /// </summary>
    public class FetchItemGoalRuntime : IQuestGoalRuntime
    {
        private readonly FetchItemGoal goal;
        private int currentCount;

        public FetchItemGoalRuntime(FetchItemGoal goal) { this.goal = goal; }

        public override int CurrentCount  => currentCount;
        public override int RequiredCount => goal != null ? goal.requiredCount : 0;

        public override string Describe()
        {
            string name = goal != null && goal.targetItem != null ? goal.targetItem.displayName : "<unset item>";
            return $"Find {name}: {currentCount} / {RequiredCount}";
        }

        public override bool OnItemPickedUp(ItemPickedUpEvent evt)
        {
            if (goal == null || goal.targetItem == null) return false;
            if (evt.Item != goal.targetItem) return false;
            if (currentCount >= goal.requiredCount) return false;
            currentCount++;
            return true;
        }
    }
}
