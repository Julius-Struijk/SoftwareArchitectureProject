using UnityEngine;
using CMGTSA.Core;

namespace CMGTSA.Inventory
{
    /// <summary>
    /// Published by <see cref="LootDropper"/> for each rolled drop on enemy death.
    /// Subscribers: <see cref="WorldItemSpawner"/> (instantiates the pickup prefab),
    /// drop VFX (slice 7), loot magnet (slice 7).
    /// </summary>
    public readonly struct ItemDroppedEvent : IGameEvent
    {
        public readonly ItemData Item;
        public readonly Vector3 Position;

        public ItemDroppedEvent(ItemData item, Vector3 position)
        {
            Item = item;
            Position = position;
        }
    }
}
