namespace CMGTSA.Inventory
{
    /// <summary>
    /// Looks up the <see cref="IItemUseStrategy"/> for an item's <see cref="ItemCategory"/>.
    /// Behind an interface so tests can inject a fake without constructing the real one.
    /// </summary>
    public interface IItemUseStrategyRegistry
    {
        /// <summary>
        /// Returns the strategy for the given category, or a no-op fallback if unmapped
        /// (so an unknown category never throws — it simply does nothing).
        /// </summary>
        IItemUseStrategy GetStrategy(ItemCategory category);
    }
}
