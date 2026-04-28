using System;
using System.Collections.Generic;

namespace CMGTSA.Inventory
{
    /// <summary>Alphabetical, A→Z by <see cref="ItemData.displayName"/>.</summary>
    public class NameAscendingSort : IInventorySortStrategy
    {
        public void Sort(List<InventorySlot> slots) =>
            slots.Sort((a, b) => string.Compare(
                a.Item.displayName, b.Item.displayName,
                StringComparison.OrdinalIgnoreCase));
    }
}
