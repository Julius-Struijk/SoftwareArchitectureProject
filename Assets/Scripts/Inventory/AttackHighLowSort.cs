using System.Collections.Generic;

namespace CMGTSA.Inventory
{
    /// <summary>By <see cref="ItemData.attackBonus"/> high→low. Ties broken by name A→Z.</summary>
    public class AttackHighLowSort : IInventorySortStrategy
    {
        public void Sort(List<InventorySlot> slots) =>
            slots.Sort((a, b) =>
            {
                int byAttack = b.Item.attackBonus.CompareTo(a.Item.attackBonus);
                if (byAttack != 0) return byAttack;
                return string.Compare(a.Item.displayName, b.Item.displayName,
                    System.StringComparison.OrdinalIgnoreCase);
            });
    }
}
