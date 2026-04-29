using UnityEngine;
using CMGTSA.Inventory;

namespace CMGTSA.Quests
{
    /// <summary>
    /// Concrete <see cref="SOQuestGoal"/>: completes when <see cref="requiredCount"/> instances
    /// of <see cref="targetItem"/> have been picked up. Identity is the <c>ItemData</c> SO
    /// reference (matches how <c>ItemPickedUpEvent.Item</c> is filled by <c>ItemPickup</c>).
    /// </summary>
    [CreateAssetMenu(fileName = "FetchItemGoal", menuName = "Scriptable Objects/Quests/FetchItemGoal")]
    public class FetchItemGoal : SOQuestGoal
    {
        [Tooltip("ItemData SO whose pickups count toward this goal. Compared by reference equality.")]
        public ItemData targetItem;

        [Tooltip("How many of targetItem must be picked up for this goal to complete.")]
        [Min(1)] public int requiredCount = 1;

        public override string Describe()
        {
            string name = targetItem != null ? targetItem.displayName : "<unset item>";
            return $"Find {requiredCount} {name}";
        }

        public override IQuestGoalRuntime CreateRuntime()
        {
            return new FetchItemGoalRuntime(this);
        }
    }
}
