using NUnit.Framework;
using UnityEngine;
using CMGTSA.Inventory;

namespace CMGTSA.Tests
{
    public class ItemUseStrategyTests
    {
        private class FakeContext : IItemUseContext
        {
            public int healCalls;
            public int lastHealAmount;
            public int applyPassiveCalls;
            public ItemData lastPassiveItem;

            public void Heal(int amount) { healCalls++; lastHealAmount = amount; }
            public void ApplyPassive(ItemData item) { applyPassiveCalls++; lastPassiveItem = item; }
        }

        private static ItemData MakeItem(int healAmount, ItemCategory category)
        {
            var item = ScriptableObject.CreateInstance<ItemData>();
            item.displayName = "Test Item";
            item.consumableHealAmount = healAmount;
            item.category = category;
            return item;
        }

        [Test]
        public void Consumable_calls_Heal_and_returns_Consumed()
        {
            var item = MakeItem(healAmount: 4, category: null);
            var ctx  = new FakeContext();
            var strat = new ConsumableUseStrategy();

            var effect = strat.Apply(item, ctx);

            Assert.AreEqual(ItemUseEffect.Consumed, effect);
            Assert.AreEqual(1, ctx.healCalls);
            Assert.AreEqual(4, ctx.lastHealAmount);
            Object.DestroyImmediate(item);
        }

        [Test]
        public void PassiveEquip_calls_ApplyPassive_and_returns_ToggledEquip()
        {
            var item = MakeItem(healAmount: 0, category: null);
            var ctx  = new FakeContext();
            var strat = new PassiveEquipStrategy();

            var effect = strat.Apply(item, ctx);

            Assert.AreEqual(ItemUseEffect.ToggledEquip, effect);
            Assert.AreEqual(1, ctx.applyPassiveCalls);
            Assert.AreSame(item, ctx.lastPassiveItem);
            Object.DestroyImmediate(item);
        }

        [Test]
        public void QuestItem_returns_NoEffect_and_touches_nothing()
        {
            var item = MakeItem(healAmount: 999, category: null);
            var ctx  = new FakeContext();
            var strat = new QuestItemUseStrategy();

            var effect = strat.Apply(item, ctx);

            Assert.AreEqual(ItemUseEffect.NoEffect, effect);
            Assert.AreEqual(0, ctx.healCalls);
            Assert.AreEqual(0, ctx.applyPassiveCalls);
            Object.DestroyImmediate(item);
        }
    }
}
