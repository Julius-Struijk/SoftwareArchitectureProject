using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using CMGTSA.Core;
using CMGTSA.Enemies;
using CMGTSA.Inventory;
using CMGTSA.Quests;

namespace CMGTSA.Tests
{
    public class QuestManagerTests
    {
        private EnemyData ghost;
        private ItemData oldKey;
        private KillEnemyGoal killTwoGhosts;
        private FetchItemGoal fetchOneKey;
        private QuestData questCull;
        private QuestData questFetch;

        [SetUp]
        public void SetUp()
        {
            EventBus<QuestStartedEvent>.Clear();
            EventBus<QuestProgressEvent>.Clear();
            EventBus<QuestCompletedEvent>.Clear();

            ghost  = ScriptableObject.CreateInstance<EnemyData>();
            ghost.name = "Ghost";
            oldKey = ScriptableObject.CreateInstance<ItemData>();
            oldKey.displayName = "Old Key";

            killTwoGhosts = ScriptableObject.CreateInstance<KillEnemyGoal>();
            killTwoGhosts.targetEnemy = ghost;
            killTwoGhosts.requiredCount = 2;

            fetchOneKey = ScriptableObject.CreateInstance<FetchItemGoal>();
            fetchOneKey.targetItem = oldKey;
            fetchOneKey.requiredCount = 1;

            questCull = ScriptableObject.CreateInstance<QuestData>();
            questCull.displayName = "Cull";
            questCull.goals = new SOQuestGoal[] { killTwoGhosts };
            questCull.xpReward = 5;

            questFetch = ScriptableObject.CreateInstance<QuestData>();
            questFetch.displayName = "Fetch";
            questFetch.goals = new SOQuestGoal[] { fetchOneKey };
            questFetch.xpReward = 7;
        }

        [TearDown]
        public void TearDown()
        {
            EventBus<QuestStartedEvent>.Clear();
            EventBus<QuestProgressEvent>.Clear();
            EventBus<QuestCompletedEvent>.Clear();
            Object.DestroyImmediate(questCull);
            Object.DestroyImmediate(questFetch);
            Object.DestroyImmediate(killTwoGhosts);
            Object.DestroyImmediate(fetchOneKey);
            Object.DestroyImmediate(ghost);
            Object.DestroyImmediate(oldKey);
        }

        [Test]
        public void AssignStartingQuests_publishes_Started_per_quest_and_populates_active_list()
        {
            var core = new QuestManager.PureCore();
            var started = new List<QuestStartedEvent>();
            EventBus<QuestStartedEvent>.Subscribe(started.Add);

            core.AssignStartingQuests(new[] { questCull, questFetch });

            Assert.AreEqual(2, started.Count);
            Assert.AreSame(questCull,  started[0].Quest);
            Assert.AreSame(questFetch, started[1].Quest);
            Assert.AreEqual(2, core.Active.Count);
        }

        [Test]
        public void Reassign_clears_previous_active_quests()
        {
            var core = new QuestManager.PureCore();
            core.AssignStartingQuests(new[] { questCull });
            core.AssignStartingQuests(new[] { questFetch });

            Assert.AreEqual(1, core.Active.Count);
            Assert.AreSame(questFetch, core.Active[0].Data);
        }

        [Test]
        public void Matching_kill_publishes_Progress_but_not_Completed_until_required_count()
        {
            var core = new QuestManager.PureCore();
            core.AssignStartingQuests(new[] { questCull });
            int progressCount = 0; int completedCount = 0;
            EventBus<QuestProgressEvent>.Subscribe(_ => progressCount++);
            EventBus<QuestCompletedEvent>.Subscribe(_ => completedCount++);

            core.HandleEnemyDied(new EnemyDiedEvent(0, 0, Vector3.zero, ghost));

            Assert.AreEqual(1, progressCount);
            Assert.AreEqual(0, completedCount);
        }

        [Test]
        public void Reaching_required_count_publishes_Completed_with_xp_reward()
        {
            var core = new QuestManager.PureCore();
            core.AssignStartingQuests(new[] { questCull });
            QuestCompletedEvent? completed = null;
            EventBus<QuestCompletedEvent>.Subscribe(e => completed = e);

            core.HandleEnemyDied(new EnemyDiedEvent(0, 0, Vector3.zero, ghost));
            core.HandleEnemyDied(new EnemyDiedEvent(0, 0, Vector3.zero, ghost));

            Assert.IsTrue(completed.HasValue);
            Assert.AreSame(questCull, completed.Value.Quest);
            Assert.AreEqual(5, completed.Value.XPReward);
        }

        [Test]
        public void Completed_publishes_exactly_once_even_with_extra_kills()
        {
            var core = new QuestManager.PureCore();
            core.AssignStartingQuests(new[] { questCull });
            int completedCount = 0;
            EventBus<QuestCompletedEvent>.Subscribe(_ => completedCount++);

            core.HandleEnemyDied(new EnemyDiedEvent(0, 0, Vector3.zero, ghost));
            core.HandleEnemyDied(new EnemyDiedEvent(0, 0, Vector3.zero, ghost));
            core.HandleEnemyDied(new EnemyDiedEvent(0, 0, Vector3.zero, ghost));
            core.HandleEnemyDied(new EnemyDiedEvent(0, 0, Vector3.zero, ghost));

            Assert.AreEqual(1, completedCount);
        }

        [Test]
        public void Pickup_event_drives_fetch_goal_completion()
        {
            var core = new QuestManager.PureCore();
            core.AssignStartingQuests(new[] { questFetch });
            QuestCompletedEvent? completed = null;
            EventBus<QuestCompletedEvent>.Subscribe(e => completed = e);

            core.HandleItemPickedUp(new ItemPickedUpEvent(oldKey, Vector3.zero));

            Assert.IsTrue(completed.HasValue);
            Assert.AreSame(questFetch, completed.Value.Quest);
            Assert.AreEqual(7, completed.Value.XPReward);
        }

        [Test]
        public void Non_matching_event_publishes_nothing()
        {
            var core = new QuestManager.PureCore();
            core.AssignStartingQuests(new[] { questCull });
            int progressCount = 0;
            EventBus<QuestProgressEvent>.Subscribe(_ => progressCount++);

            // Pickup event has nothing to do with the kill quest
            core.HandleItemPickedUp(new ItemPickedUpEvent(oldKey, Vector3.zero));

            Assert.AreEqual(0, progressCount);
        }

        [Test]
        public void Null_quest_in_starter_array_is_skipped()
        {
            var core = new QuestManager.PureCore();
            int started = 0;
            EventBus<QuestStartedEvent>.Subscribe(_ => started++);

            core.AssignStartingQuests(new QuestData[] { null, questCull, null });

            Assert.AreEqual(1, started);
            Assert.AreEqual(1, core.Active.Count);
        }
    }
}
