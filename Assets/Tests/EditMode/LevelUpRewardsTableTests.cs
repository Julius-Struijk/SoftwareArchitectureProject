using NUnit.Framework;
using UnityEngine;
using CMGTSA.Player;

namespace CMGTSA.Tests
{
    public class LevelUpRewardsTableTests
    {
        private LevelUpRewardsTable table;

        [SetUp]
        public void SetUp()
        {
            table = ScriptableObject.CreateInstance<LevelUpRewardsTable>();
            table.rewards = new[]
            {
                new LevelUpRewardsTable.LevelReward { level = 2, hpDelta = 2, damageDelta = 1, healToFull = true  },
                new LevelUpRewardsTable.LevelReward { level = 3, hpDelta = 2, damageDelta = 1, healToFull = true  },
                new LevelUpRewardsTable.LevelReward { level = 5, hpDelta = 4, damageDelta = 2, healToFull = true  },
            };
            table.fallback = new LevelUpRewardsTable.LevelReward
            {
                level = 0, hpDelta = 1, damageDelta = 0, healToFull = false,
            };
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(table);
        }

        [Test]
        public void Get_returns_matching_entry_for_known_level()
        {
            var r = table.Get(2);

            Assert.AreEqual(2, r.level);
            Assert.AreEqual(2, r.hpDelta);
            Assert.AreEqual(1, r.damageDelta);
            Assert.IsTrue(r.healToFull);
        }

        [Test]
        public void Get_finds_each_distinct_entry()
        {
            Assert.AreEqual(2, table.Get(3).hpDelta);
            Assert.AreEqual(4, table.Get(5).hpDelta);
        }

        [Test]
        public void Get_returns_fallback_when_level_not_in_table()
        {
            var r = table.Get(4);

            Assert.AreEqual(0, r.level, "Fallback was authored with level=0.");
            Assert.AreEqual(1, r.hpDelta);
            Assert.AreEqual(0, r.damageDelta);
            Assert.IsFalse(r.healToFull);
        }

        [Test]
        public void Get_returns_fallback_when_level_past_table()
        {
            var r = table.Get(99);

            Assert.AreEqual(1, r.hpDelta, "Past-table levels use the fallback row.");
        }

        [Test]
        public void Get_with_null_rewards_array_returns_fallback()
        {
            var freshTable = ScriptableObject.CreateInstance<LevelUpRewardsTable>();
            freshTable.fallback = new LevelUpRewardsTable.LevelReward
            {
                level = 0, hpDelta = 7, damageDelta = 0, healToFull = false,
            };

            var r = freshTable.Get(2);

            Assert.AreEqual(7, r.hpDelta);

            Object.DestroyImmediate(freshTable);
        }
    }
}
