using System.Collections.Generic;

namespace CMGTSA.Inventory
{
    /// <summary>
    /// Strategy: how to order the slot list when the model mutates. Implementations
    /// sort in place. Pure functions of the slot list — no side-effects.
    /// </summary>
    public interface IInventorySortStrategy
    {
        void Sort(List<InventorySlot> slots);
    }
}
