using System.Collections.Generic;

namespace CMGTSA.Inventory
{
    /// <summary>
    /// Concrete registry. Built from a list of (category, strategy) pairs at construction.
    /// Returns <see cref="QuestItemUseStrategy"/> as the safe no-op fallback for unmapped
    /// categories (e.g. a designer adds a new category SO but forgets to register it).
    /// </summary>
    public class ItemUseStrategyRegistry : IItemUseStrategyRegistry
    {
        private readonly Dictionary<ItemCategory, IItemUseStrategy> map =
            new Dictionary<ItemCategory, IItemUseStrategy>();
        private static readonly IItemUseStrategy fallback = new QuestItemUseStrategy();

        public ItemUseStrategyRegistry(IEnumerable<(ItemCategory, IItemUseStrategy)> entries)
        {
            foreach (var (cat, strat) in entries)
            {
                if (cat == null || strat == null) continue;
                map[cat] = strat;
            }
        }

        public IItemUseStrategy GetStrategy(ItemCategory category)
        {
            if (category == null) return fallback;
            return map.TryGetValue(category, out var s) ? s : fallback;
        }
    }
}
