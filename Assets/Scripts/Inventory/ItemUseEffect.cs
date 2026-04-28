namespace CMGTSA.Inventory
{
    /// <summary>
    /// What kind of side-effect a use produced inside the inventory:
    /// <c>Consumed</c> – decrement the slot count (and remove if it hits zero).
    /// <c>ToggledEquip</c> – flip the slot's <c>Equipped</c> flag; do not remove.
    /// <c>NoEffect</c> – use was not legal (e.g. a quest item the player tried to use directly).
    /// The strategy returns this value; <see cref="InventoryModel.UseSlot(int)"/> applies it.
    /// </summary>
    public enum ItemUseEffect
    {
        NoEffect = 0,
        Consumed = 1,
        ToggledEquip = 2
    }
}
