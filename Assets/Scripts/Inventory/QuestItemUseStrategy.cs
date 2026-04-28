namespace CMGTSA.Inventory
{
    /// <summary>
    /// Strategy for quest items (e.g. keys, plot items). The player cannot use them directly;
    /// they are consumed by quest goal subscribers in slice 4. Returns
    /// <see cref="ItemUseEffect.NoEffect"/> so the slot stays untouched.
    /// </summary>
    public class QuestItemUseStrategy : IItemUseStrategy
    {
        public ItemUseEffect Apply(ItemData item, IItemUseContext context)
        {
            _ = item;
            _ = context;
            return ItemUseEffect.NoEffect;
        }
    }
}
