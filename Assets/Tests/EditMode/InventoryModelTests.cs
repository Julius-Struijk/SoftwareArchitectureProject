using NUnit.Framework;
using UnityEngine;
using CMGTSA.Core;
using CMGTSA.Inventory;

namespace CMGTSA.Tests
{
    public class InventoryModelTests
    {
        private ItemCategory consumableCat;
        private ItemCategory passiveCat;
        private ItemCategory questCat;
        private ItemUseStrategyRegistry registry;
        private FakeContext ctx;

        private class FakeContext : IItemUseContext
        {
            public int healCalls; public int lastHeal;
            public int passiveCalls; public ItemData lastPassive;
            public void Heal(int a) { healCalls++; lastHeal = a; }
            public void ApplyPassive(ItemData i) { passiveCalls++; lastPassive = i; }
        }

        [SetUp]
        public void SetUp()
        {
            EventBus<ItemPickedUpEvent>.Clear();
            EventBus<ItemUsedEvent>.Clear();

            consumableCat = ScriptableObject.CreateInstance<ItemCategory>();
            consumableCat.displayName = "Consumable";
            passiveCat    = ScriptableObject.CreateInstance<ItemCategory>();
            passiveCat.displayName = "Passive";
            questCat      = ScriptableObject.CreateInstance<ItemCategory>();
            questCat.displayName = "Quest";

            registry = new ItemUseStrategyRegistry(new[]
            {
                (consumableCat, (IItemUseStrategy)new ConsumableUseStrategy()),
                (passiveCat,    (IItemUseStrategy)new PassiveEquipStrategy()),
                (questCat,      (IItemUseStrategy)new QuestItemUseStrategy()),
            });

            ctx = new FakeContext();
        }

        [TearDown]
        public void TearDown()
        {
            EventBus<ItemPickedUpEvent>.Clear();
            EventBus<ItemUsedEvent>.Clear();
            Object.DestroyImmediate(consumableCat);
            Object.DestroyImmediate(passiveCat);
            Object.DestroyImmediate(questCat);
        }

        private ItemData MakeItem(string name, ItemCategory cat, bool stackable,
                                  int heal = 0, int attack = 0)
        {
            var i = ScriptableObject.CreateInstance<ItemData>();
            i.displayName = name;
            i.category = cat;
            i.isStackable = stackable;
            i.consumableHealAmount = heal;
            i.attackBonus = attack;
            return i;
        }

        [Test]
        public void Constructor_starts_empty()
        {
            var inv = new InventoryModel(ctx, registry);
            Assert.AreEqual(0, inv.Slots.Count);
        }

        [Test]
        public void Add_stackable_twice_yields_one_slot_count_two()
        {
            var inv = new InventoryModel(ctx, registry);
            var potion = MakeItem("Potion", consumableCat, stackable: true, heal: 3);

            inv.Add(potion);
            inv.Add(potion);

            Assert.AreEqual(1, inv.Slots.Count);
            Assert.AreEqual(2, inv.Slots[0].Count);
            Object.DestroyImmediate(potion);
        }

        [Test]
        public void Add_non_stackable_twice_yields_two_slots()
        {
            var inv = new InventoryModel(ctx, registry);
            var charm = MakeItem("Charm", passiveCat, stackable: false);

            inv.Add(charm);
            inv.Add(charm);

            Assert.AreEqual(2, inv.Slots.Count);
            Assert.AreEqual(1, inv.Slots[0].Count);
            Assert.AreEqual(1, inv.Slots[1].Count);
            Object.DestroyImmediate(charm);
        }

        [Test]
        public void Add_fires_OnChanged_observer()
        {
            var inv = new InventoryModel(ctx, registry);
            var item = MakeItem("X", consumableCat, stackable: true);
            int changes = 0;
            inv.OnChanged += () => changes++;

            inv.Add(item);

            Assert.AreEqual(1, changes);
            Object.DestroyImmediate(item);
        }

        [Test]
        public void UseSlot_consumable_decrements_count_and_calls_Heal()
        {
            var inv = new InventoryModel(ctx, registry);
            var potion = MakeItem("Potion", consumableCat, stackable: true, heal: 3);
            inv.Add(potion);
            inv.Add(potion);   // count = 2
            int usedEvents = 0;
            EventBus<ItemUsedEvent>.Subscribe(_ => usedEvents++);

            inv.UseSlot(0);

            Assert.AreEqual(1, inv.Slots[0].Count);
            Assert.AreEqual(1, ctx.healCalls);
            Assert.AreEqual(3, ctx.lastHeal);
            Assert.AreEqual(1, usedEvents);
            Object.DestroyImmediate(potion);
        }

        [Test]
        public void UseSlot_consumable_at_count_one_removes_slot()
        {
            var inv = new InventoryModel(ctx, registry);
            var potion = MakeItem("Potion", consumableCat, stackable: true, heal: 3);
            inv.Add(potion);

            inv.UseSlot(0);

            Assert.AreEqual(0, inv.Slots.Count);
            Object.DestroyImmediate(potion);
        }

        [Test]
        public void UseSlot_passive_toggles_Equipped_and_does_not_remove()
        {
            var inv = new InventoryModel(ctx, registry);
            var charm = MakeItem("Charm", passiveCat, stackable: false);
            inv.Add(charm);

            inv.UseSlot(0);

            Assert.AreEqual(1, inv.Slots.Count, "Equip toggle does not consume");
            Assert.IsTrue(inv.Slots[0].Equipped);
            Assert.AreEqual(1, ctx.passiveCalls);

            inv.UseSlot(0);

            Assert.IsFalse(inv.Slots[0].Equipped, "Second use unequips");
            Object.DestroyImmediate(charm);
        }

        [Test]
        public void UseSlot_quest_item_does_nothing()
        {
            var inv = new InventoryModel(ctx, registry);
            var key = MakeItem("Key", questCat, stackable: false);
            inv.Add(key);
            int usedEvents = 0;
            EventBus<ItemUsedEvent>.Subscribe(_ => usedEvents++);

            inv.UseSlot(0);

            Assert.AreEqual(1, inv.Slots.Count);
            Assert.IsFalse(inv.Slots[0].Equipped);
            Assert.AreEqual(0, usedEvents, "ItemUsedEvent only fires when something happened");
            Object.DestroyImmediate(key);
        }

        [Test]
        public void UseSlot_with_invalid_index_is_a_noop()
        {
            var inv = new InventoryModel(ctx, registry);
            inv.UseSlot(0);
            inv.UseSlot(-1);
            inv.UseSlot(99);
            Assert.AreEqual(0, inv.Slots.Count);
        }

        [Test]
        public void SetSortStrategy_re_sorts_existing_slots()
        {
            var inv = new InventoryModel(ctx, registry);
            var a = MakeItem("Apple", consumableCat, stackable: false);
            var z = MakeItem("Zebra", consumableCat, stackable: false);
            inv.Add(z);
            inv.Add(a);

            inv.SetSortStrategy(new NameAscendingSort());

            Assert.AreEqual("Apple", inv.Slots[0].Item.displayName);
            Assert.AreEqual("Zebra", inv.Slots[1].Item.displayName);
            Object.DestroyImmediate(a); Object.DestroyImmediate(z);
        }
    }
}
