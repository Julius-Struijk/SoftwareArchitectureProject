namespace CMGTSA.Inventory
{
    /// <summary>
    /// Strategy: per-category use behaviour. Stateless. The model picks a strategy by
    /// looking the item's <see cref="ItemCategory"/> up in <see cref="IItemUseStrategyRegistry"/>,
    /// calls <see cref="Apply"/>, and reads the returned <see cref="ItemUseEffect"/> to
    /// know whether to decrement the slot, toggle equip, or do nothing.
    /// </summary>
    public interface IItemUseStrategy
    {
        ItemUseEffect Apply(ItemData item, IItemUseContext context);
    }
}
