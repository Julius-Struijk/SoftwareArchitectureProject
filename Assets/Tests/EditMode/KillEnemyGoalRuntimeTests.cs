using NUnit.Framework;
using UnityEngine;
using CMGTSA.Enemies;
using CMGTSA.Quests;

namespace CMGTSA.Tests
{
    public class KillEnemyGoalRuntimeTests
    {
        private EnemyData ghost;
        private EnemyData golem;
        private KillEnemyGoal goal;

        [SetUp]
        public void SetUp()
        {
            ghost = ScriptableObject.CreateInstance<EnemyData>();
            ghost.name = "Ghost";
            golem = ScriptableObject.CreateInstance<EnemyData>();
            golem.name = "Golem";

            goal = ScriptableObject.CreateInstance<KillEnemyGoal>();
            goal.targetEnemy = ghost;
            goal.requiredCount = 2;
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(goal);
            Object.DestroyImmediate(ghost);
            Object.DestroyImmediate(golem);
        }

        [Test]
        public void New_runtime_starts_at_zero_and_not_complete()
        {
            var rt = goal.CreateRuntime();
            Assert.AreEqual(0, rt.CurrentCount);
            Assert.AreEqual(2, rt.RequiredCount);
            Assert.IsFalse(rt.IsComplete);
        }

        [Test]
        public void Matching_kill_increments_and_returns_true()
        {
            var rt = goal.CreateRuntime();
            bool changed = rt.OnEnemyDied(new EnemyDiedEvent(0, 0, Vector3.zero, ghost));
            Assert.IsTrue(changed);
            Assert.AreEqual(1, rt.CurrentCount);
            Assert.IsFalse(rt.IsComplete);
        }

        [Test]
        public void Non_matching_kill_does_nothing_and_returns_false()
        {
            var rt = goal.CreateRuntime();
            bool changed = rt.OnEnemyDied(new EnemyDiedEvent(0, 0, Vector3.zero, golem));
            Assert.IsFalse(changed);
            Assert.AreEqual(0, rt.CurrentCount);
        }

        [Test]
        public void Reaching_required_count_marks_complete()
        {
            var rt = goal.CreateRuntime();
            rt.OnEnemyDied(new EnemyDiedEvent(0, 0, Vector3.zero, ghost));
            rt.OnEnemyDied(new EnemyDiedEvent(0, 0, Vector3.zero, ghost));
            Assert.AreEqual(2, rt.CurrentCount);
            Assert.IsTrue(rt.IsComplete);
        }

        [Test]
        public void Extra_kills_after_complete_do_not_over_count()
        {
            var rt = goal.CreateRuntime();
            rt.OnEnemyDied(new EnemyDiedEvent(0, 0, Vector3.zero, ghost));
            rt.OnEnemyDied(new EnemyDiedEvent(0, 0, Vector3.zero, ghost));
            bool changed = rt.OnEnemyDied(new EnemyDiedEvent(0, 0, Vector3.zero, ghost));
            Assert.IsFalse(changed);
            Assert.AreEqual(2, rt.CurrentCount);
        }
    }
}
