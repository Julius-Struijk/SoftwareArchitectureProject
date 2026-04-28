using System;
using System.Collections.Generic;

namespace CMGTSA.Inventory
{
    /// <summary>Alphabetical, Z→A by <see cref="ItemData.displayName"/>.</summary>
    public class NameDescendingSort : IInventorySortStrategy
    {
        public void Sort(List<InventorySlot> slots) =>
            slots.Sort((a, b) => string.Compare(
                b.Item.displayName, a.Item.displayName,
                StringComparison.OrdinalIgnoreCase));
    }
}
