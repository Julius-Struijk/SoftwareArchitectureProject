namespace CMGTSA.Inventory
{
    /// <summary>
    /// Strategy for passive-equip items (e.g. charms). Asks the host context to apply the
    /// passive (slice 5 wires real stat buffs) and returns <see cref="ItemUseEffect.ToggledEquip"/>
    /// so the inventory model flips the slot's <c>Equipped</c> flag without removing it.
    /// </summary>
    public class PassiveEquipStrategy : IItemUseStrategy
    {
        public ItemUseEffect Apply(ItemData item, IItemUseContext context)
        {
            if (item == null || context == null) return ItemUseEffect.NoEffect;
            context.ApplyPassive(item);
            return ItemUseEffect.ToggledEquip;
        }
    }
}
