using CMGTSA.Core;

namespace CMGTSA.Inventory
{
    /// <summary>
    /// Published by <see cref="InventoryModel"/> after a strategy successfully applies.
    /// Subscribers: HUD toast (slice 7), QuestManager (slice 4), audio feedback.
    /// </summary>
    public readonly struct ItemUsedEvent : IGameEvent
    {
        public readonly ItemData Item;
        public readonly ItemUseEffect Effect;

        public ItemUsedEvent(ItemData item, ItemUseEffect effect)
        {
            Item = item;
            Effect = effect;
        }
    }
}
