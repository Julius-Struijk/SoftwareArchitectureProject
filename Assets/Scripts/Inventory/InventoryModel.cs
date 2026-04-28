using System;
using System.Collections.Generic;
using CMGTSA.Core;

namespace CMGTSA.Inventory
{
    /// <summary>
    /// The M in the inventory's MVP. Plain C# — owned by <c>PlayerController</c>, testable
    /// in EditMode without a scene. Holds an ordered <see cref="InventorySlot"/> list and
    /// publishes <see cref="ItemPickedUpEvent"/> + <see cref="ItemUsedEvent"/> on the bus
    /// for cross-system listeners (slice 4 quests, slice 7 toast). Exposes a per-instance
    /// <see cref="OnChanged"/> Action for the presenter — Observer pattern, intra-system
    /// one-to-many, keeping per-frame UI refreshes off the bus.
    /// </summary>
    public class InventoryModel
    {
        private readonly List<InventorySlot> slots = new List<InventorySlot>();
        private readonly IItemUseContext useContext;
        private readonly IItemUseStrategyRegistry registry;
        private IInventorySortStrategy sortStrategy = new ObtainedOrderSort();
        private int orderCounter;

        public IReadOnlyList<InventorySlot> Slots => slots;
        public event Action OnChanged;

        public InventoryModel(IItemUseContext useContext, IItemUseStrategyRegistry registry)
        {
            this.useContext = useContext;
            this.registry = registry;
        }

        public void Add(ItemData item)
        {
            if (item == null) return;

            if (item.isStackable)
            {
                InventorySlot existing = null;
                for (int i = 0; i < slots.Count; i++)
                {
                    if (slots[i].Item == item) { existing = slots[i]; break; }
                }
                if (existing != null)
                {
                    existing.IncrementCount();
                }
                else
                {
                    slots.Add(new InventorySlot(item, 1, orderCounter++));
                }
            }
            else
            {
                slots.Add(new InventorySlot(item, 1, orderCounter++));
            }

            sortStrategy.Sort(slots);
            OnChanged?.Invoke();
        }

        public void UseSlot(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= slots.Count) return;
            var slot = slots[slotIndex];
            var strategy = registry.GetStrategy(slot.Item.category);
            var effect = strategy.Apply(slot.Item, useContext);

            switch (effect)
            {
                case ItemUseEffect.Consumed:
                    slot.DecrementCount();
                    if (slot.Count <= 0) slots.RemoveAt(slotIndex);
                    break;
                case ItemUseEffect.ToggledEquip:
                    slot.SetEquipped(!slot.Equipped);
                    break;
                case ItemUseEffect.NoEffect:
                    return; // do not publish ItemUsedEvent or invoke OnChanged
            }

            sortStrategy.Sort(slots);
            EventBus<ItemUsedEvent>.Publish(new ItemUsedEvent(slot.Item, effect));
            OnChanged?.Invoke();
        }

        public void SetSortStrategy(IInventorySortStrategy newStrategy)
        {
            if (newStrategy == null) return;
            sortStrategy = newStrategy;
            sortStrategy.Sort(slots);
            OnChanged?.Invoke();
        }
    }
}
