using UnityEngine;

namespace CMGTSA.Inventory
{
    /// <summary>
    /// Enhanced enum: a ScriptableObject acting as an enum value. The asset reference
    /// itself is the identity (compare with <c>==</c> or <c>ReferenceEquals</c>); the
    /// fields are designer-friendly metadata. Three assets are created in slice 3:
    /// Consumable, PassiveEquip, QuestItem. <see cref="ItemUseStrategyRegistry"/> maps
    /// each asset to one <see cref="IItemUseStrategy"/> instance.
    /// </summary>
    [CreateAssetMenu(fileName = "ItemCategory", menuName = "Scriptable Objects/Inventory/ItemCategory")]
    public class ItemCategory : ScriptableObject
    {
        [Tooltip("Player-facing label shown in tooltips and the inventory panel header.")]
        public string displayName;

        [Tooltip("Color tint applied to the slot border in the inventory UI. Optional.")]
        public Color tint = Color.white;
    }
}
