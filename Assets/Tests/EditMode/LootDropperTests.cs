using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using CMGTSA.Core;
using CMGTSA.Enemies;
using CMGTSA.Inventory;

namespace CMGTSA.Tests
{
    public class LootDropperTests
    {
        [SetUp]
        public void SetUp()
        {
            EventBus<EnemyDiedEvent>.Clear();
            EventBus<ItemDroppedEvent>.Clear();
        }

        [TearDown]
        public void TearDown()
        {
            EventBus<EnemyDiedEvent>.Clear();
            EventBus<ItemDroppedEvent>.Clear();
        }

        [Test]
        public void Rolls_each_entry_and_publishes_drops_for_passing_rolls()
        {
            var item1 = ScriptableObject.CreateInstance<ItemData>(); item1.displayName = "I1";
            var item2 = ScriptableObject.CreateInstance<ItemData>(); item2.displayName = "I2";
            var enemyData = ScriptableObject.CreateInstance<EnemyData>();
            enemyData.lootTable = new[]
            {
                new LootEntry { item = item1, chance = 0.5f },
                new LootEntry { item = item2, chance = 0.5f },
            };

            // Deterministic rolls: first entry passes (0.4 < 0.5), second fails (0.9 > 0.5).
            var rolls = new Queue<float>(new[] { 0.4f, 0.9f });
            var dropper = new LootDropper.PureCore(() => rolls.Dequeue());

            var drops = new List<ItemDroppedEvent>();
            EventBus<ItemDroppedEvent>.Subscribe(drops.Add);

            dropper.RollAndPublish(enemyData, new Vector3(7, 8, 0));

            Assert.AreEqual(1, drops.Count);
            Assert.AreSame(item1, drops[0].Item);
            Assert.AreEqual(new Vector3(7, 8, 0), drops[0].Position);

            Object.DestroyImmediate(item1);
            Object.DestroyImmediate(item2);
            Object.DestroyImmediate(enemyData);
        }

        [Test]
        public void Empty_loot_table_publishes_nothing()
        {
            var enemyData = ScriptableObject.CreateInstance<EnemyData>();
            enemyData.lootTable = new LootEntry[0];
            var dropper = new LootDropper.PureCore(() => 0f);
            int count = 0;
            EventBus<ItemDroppedEvent>.Subscribe(_ => count++);

            dropper.RollAndPublish(enemyData, Vector3.zero);

            Assert.AreEqual(0, count);
            Object.DestroyImmediate(enemyData);
        }

        [Test]
        public void Entries_with_null_item_are_skipped()
        {
            var enemyData = ScriptableObject.CreateInstance<EnemyData>();
            enemyData.lootTable = new[] { new LootEntry { item = null, chance = 1f } };
            var dropper = new LootDropper.PureCore(() => 0f);
            int count = 0;
            EventBus<ItemDroppedEvent>.Subscribe(_ => count++);

            dropper.RollAndPublish(enemyData, Vector3.zero);

            Assert.AreEqual(0, count);
            Object.DestroyImmediate(enemyData);
        }
    }
}
