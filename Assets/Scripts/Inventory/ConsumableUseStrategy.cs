namespace CMGTSA.Inventory
{
    /// <summary>
    /// Strategy for consumable items (e.g. HP potions). Calls <see cref="IItemUseContext.Heal"/>
    /// with the item's <c>consumableHealAmount</c> and returns <see cref="ItemUseEffect.Consumed"/>
    /// so the inventory model decrements the slot's count.
    /// </summary>
    public class ConsumableUseStrategy : IItemUseStrategy
    {
        public ItemUseEffect Apply(ItemData item, IItemUseContext context)
        {
            if (item == null || context == null) return ItemUseEffect.NoEffect;
            context.Heal(item.consumableHealAmount);
            return ItemUseEffect.Consumed;
        }
    }
}
