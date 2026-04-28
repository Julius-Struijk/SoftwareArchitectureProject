namespace CMGTSA.Inventory
{
    /// <summary>
    /// One row in the inventory. Plain C# — no Unity refs. Owned and mutated only by
    /// <see cref="InventoryModel"/>; presenters see it through <see cref="InventoryModel.Slots"/>
    /// as a read-only list.
    /// </summary>
    public class InventorySlot
    {
        public ItemData Item { get; }
        public int Count { get; private set; }
        public bool Equipped { get; private set; }
        public int OrderObtained { get; }

        public InventorySlot(ItemData item, int count, int orderObtained)
        {
            Item = item;
            Count = count;
            Equipped = false;
            OrderObtained = orderObtained;
        }

        // Mutators are internal-by-convention — only InventoryModel calls them.
        public void IncrementCount() { Count++; }
        public void DecrementCount() { Count--; }
        public void SetEquipped(bool value) { Equipped = value; }
    }
}
