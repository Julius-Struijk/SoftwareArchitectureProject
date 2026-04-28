using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using CMGTSA.Inventory;

namespace CMGTSA.Tests
{
    public class InventorySortTests
    {
        private ItemData a, m, z;

        [SetUp]
        public void SetUp()
        {
            a = MakeItem("Apple",  attack: 1);
            m = MakeItem("Mango",  attack: 5);
            z = MakeItem("Zebra",  attack: 3);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(a);
            Object.DestroyImmediate(m);
            Object.DestroyImmediate(z);
        }

        private static ItemData MakeItem(string name, int attack)
        {
            var i = ScriptableObject.CreateInstance<ItemData>();
            i.displayName = name;
            i.attackBonus = attack;
            return i;
        }

        private List<InventorySlot> MakeSlots()
        {
            return new List<InventorySlot>
            {
                new InventorySlot(z, 1, 0),
                new InventorySlot(a, 1, 1),
                new InventorySlot(m, 1, 2),
            };
        }

        [Test]
        public void NameAscending_sorts_A_to_Z()
        {
            var slots = MakeSlots();
            new NameAscendingSort().Sort(slots);
            Assert.AreEqual("Apple", slots[0].Item.displayName);
            Assert.AreEqual("Mango", slots[1].Item.displayName);
            Assert.AreEqual("Zebra", slots[2].Item.displayName);
        }

        [Test]
        public void NameDescending_sorts_Z_to_A()
        {
            var slots = MakeSlots();
            new NameDescendingSort().Sort(slots);
            Assert.AreEqual("Zebra", slots[0].Item.displayName);
            Assert.AreEqual("Mango", slots[1].Item.displayName);
            Assert.AreEqual("Apple", slots[2].Item.displayName);
        }

        [Test]
        public void AttackHighLow_sorts_by_attackBonus_descending()
        {
            var slots = MakeSlots();
            new AttackHighLowSort().Sort(slots);
            Assert.AreEqual(5, slots[0].Item.attackBonus);
            Assert.AreEqual(3, slots[1].Item.attackBonus);
            Assert.AreEqual(1, slots[2].Item.attackBonus);
        }

        [Test]
        public void AttackHighLow_breaks_ties_alphabetically()
        {
            var b1 = MakeItem("Bbb", attack: 7);
            var b2 = MakeItem("Aaa", attack: 7);
            var slots = new List<InventorySlot>
            {
                new InventorySlot(b1, 1, 0),
                new InventorySlot(b2, 1, 1),
            };

            new AttackHighLowSort().Sort(slots);

            Assert.AreEqual("Aaa", slots[0].Item.displayName);
            Assert.AreEqual("Bbb", slots[1].Item.displayName);
            Object.DestroyImmediate(b1); Object.DestroyImmediate(b2);
        }

        [Test]
        public void ObtainedOrder_sorts_by_OrderObtained()
        {
            // MakeSlots above assigns OrderObtained 0,1,2 in z,a,m order — so the
            // expected post-sort order is z, a, m (already correct, but the sort must
            // preserve it under any starting permutation).
            var slots = MakeSlots();
            slots.Reverse();
            new ObtainedOrderSort().Sort(slots);
            Assert.AreEqual("Zebra", slots[0].Item.displayName);
            Assert.AreEqual("Apple", slots[1].Item.displayName);
            Assert.AreEqual("Mango", slots[2].Item.displayName);
        }
    }
}
