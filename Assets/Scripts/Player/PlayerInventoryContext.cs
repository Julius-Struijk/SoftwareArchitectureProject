using UnityEngine;
using CMGTSA.Inventory;

namespace CMGTSA.Player
{
    /// <summary>
    /// Host-side glue: implements <see cref="IItemUseContext"/> by forwarding to
    /// <see cref="PlayerStatsModel"/>. Lives outside the Inventory package so the package
    /// stays self-contained — reusers wire their own context.
    /// </summary>
    public class PlayerInventoryContext : IItemUseContext
    {
        private readonly PlayerStatsModel stats;

        public PlayerInventoryContext(PlayerStatsModel stats)
        {
            this.stats = stats;
        }

        public void Heal(int amount) => stats.Heal(amount);

        public void ApplyPassive(ItemData item)
        {
            // Slice 5 wires real stat buffs; for slice 3 we only confirm the path runs.
            Debug.Log($"[Inventory] Toggled passive: {item.displayName} (attackBonus={item.attackBonus})");
        }
    }
}
