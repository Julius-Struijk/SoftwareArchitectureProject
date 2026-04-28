using UnityEngine;
using CMGTSA.Core;

namespace CMGTSA.Inventory
{
    /// <summary>
    /// Published by <see cref="ItemPickup"/> when the player overlaps a world pickup.
    /// Subscribers: <see cref="InventoryModel"/> (adds the item), QuestManager (slice 4 fetch quests).
    /// </summary>
    public readonly struct ItemPickedUpEvent : IGameEvent
    {
        public readonly ItemData Item;
        public readonly Vector3 Position;

        public ItemPickedUpEvent(ItemData item, Vector3 position)
        {
            Item = item;
            Position = position;
        }
    }
}
