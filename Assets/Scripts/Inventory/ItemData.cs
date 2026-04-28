using UnityEngine;

namespace CMGTSA.Inventory
{
    /// <summary>
    /// ScriptableObject Factory: produces inventory entries from designer-tuned defaults.
    /// One asset per item kind (e.g. <c>HPPotion.asset</c>). The asset reference itself is
    /// the identity — two slots stack only if their <see cref="ItemData"/> reference is the
    /// same and <see cref="isStackable"/> is true.
    /// </summary>
    [CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/Inventory/ItemData")]
    public class ItemData : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Player-facing label shown in tooltips and the inventory panel.")]
        public string displayName;

        [Tooltip("Sprite shown in the inventory slot and on the world pickup.")]
        public Sprite icon;

        [Tooltip("Drives strategy dispatch via ItemUseStrategyRegistry.")]
        public ItemCategory category;

        [Header("Behaviour flags")]
        [Tooltip("If true, multiple of the same ItemData asset stack into one slot with a count.")]
        public bool isStackable;

        [Header("Stats")]
        [Tooltip("Used by AttackHighLowSort and as the buff value applied by PassiveEquipStrategy in slice 5.")]
        public int attackBonus;

        [Tooltip("Hit-points restored when a consumable strategy applies this item.")]
        public int consumableHealAmount;

        [TextArea]
        [Tooltip("Designer-facing tooltip shown in the inventory panel. Optional.")]
        public string description;
    }
}
