using System.Collections.Generic;

namespace CMGTSA.Inventory
{
    /// <summary>By <see cref="InventorySlot.OrderObtained"/> ascending — first picked up first.</summary>
    public class ObtainedOrderSort : IInventorySortStrategy
    {
        public void Sort(List<InventorySlot> slots) =>
            slots.Sort((a, b) => a.OrderObtained.CompareTo(b.OrderObtained));
    }
}
