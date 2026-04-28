namespace CMGTSA.Inventory
{
    /// <summary>
    /// Side-effect surface every <see cref="IItemUseStrategy"/> may call. The Inventory
    /// package depends on this interface, never on <c>PlayerStatsModel</c>: that is the
    /// boundary that makes the package exportable. The host project (this game) ships
    /// <c>PlayerInventoryContext</c> in <c>CMGTSA.Player</c> as the only implementation.
    /// </summary>
    public interface IItemUseContext
    {
        void Heal(int amount);
        void ApplyPassive(ItemData item);
    }
}
