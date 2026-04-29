using NUnit.Framework;
using UnityEngine;
using CMGTSA.Inventory;
using CMGTSA.Quests;

namespace CMGTSA.Tests
{
    public class FetchItemGoalRuntimeTests
    {
        private ItemData oldKey;
        private ItemData hpPotion;
        private FetchItemGoal goal;

        [SetUp]
        public void SetUp()
        {
            oldKey   = ScriptableObject.CreateInstance<ItemData>();
            oldKey.displayName = "Old Key";
            hpPotion = ScriptableObject.CreateInstance<ItemData>();
            hpPotion.displayName = "HP Potion";

            goal = ScriptableObject.CreateInstance<FetchItemGoal>();
            goal.targetItem = oldKey;
            goal.requiredCount = 1;
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(goal);
            Object.DestroyImmediate(oldKey);
            Object.DestroyImmediate(hpPotion);
        }

        [Test]
        public void New_runtime_starts_at_zero_and_not_complete()
        {
            var rt = goal.CreateRuntime();
            Assert.AreEqual(0, rt.CurrentCount);
            Assert.AreEqual(1, rt.RequiredCount);
            Assert.IsFalse(rt.IsComplete);
        }

        [Test]
        public void Matching_pickup_increments_and_completes_at_one()
        {
            var rt = goal.CreateRuntime();
            bool changed = rt.OnItemPickedUp(new ItemPickedUpEvent(oldKey, Vector3.zero));
            Assert.IsTrue(changed);
            Assert.AreEqual(1, rt.CurrentCount);
            Assert.IsTrue(rt.IsComplete);
        }

        [Test]
        public void Non_matching_pickup_does_nothing()
        {
            var rt = goal.CreateRuntime();
            bool changed = rt.OnItemPickedUp(new ItemPickedUpEvent(hpPotion, Vector3.zero));
            Assert.IsFalse(changed);
            Assert.AreEqual(0, rt.CurrentCount);
        }

        [Test]
        public void Extra_pickups_after_complete_do_not_over_count()
        {
            var rt = goal.CreateRuntime();
            rt.OnItemPickedUp(new ItemPickedUpEvent(oldKey, Vector3.zero));
            bool changed = rt.OnItemPickedUp(new ItemPickedUpEvent(oldKey, Vector3.zero));
            Assert.IsFalse(changed);
            Assert.AreEqual(1, rt.CurrentCount);
        }

        [Test]
        public void OnEnemyDied_is_a_noop_for_fetch_goal()
        {
            var rt = goal.CreateRuntime();
            // Default base implementation returns false; sanity-check polymorphism boundary.
            var enemyData = ScriptableObject.CreateInstance<CMGTSA.Enemies.EnemyData>();
            bool changed = rt.OnEnemyDied(new CMGTSA.Enemies.EnemyDiedEvent(0, 0, Vector3.zero, enemyData));
            Assert.IsFalse(changed);
            Assert.AreEqual(0, rt.CurrentCount);
            Object.DestroyImmediate(enemyData);
        }
    }
}
