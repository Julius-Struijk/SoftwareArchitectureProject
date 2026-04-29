using CMGTSA.Enemies;

namespace CMGTSA.Quests
{
    /// <summary>
    /// Runtime tracker for a <see cref="KillEnemyGoal"/>. Increments <see cref="CurrentCount"/>
    /// each time <see cref="EnemyDiedEvent.Source"/> matches the goal's target.
    /// </summary>
    public class KillEnemyGoalRuntime : IQuestGoalRuntime
    {
        private readonly KillEnemyGoal goal;
        private int currentCount;

        public KillEnemyGoalRuntime(KillEnemyGoal goal) { this.goal = goal; }

        public override int CurrentCount  => currentCount;
        public override int RequiredCount => goal != null ? goal.requiredCount : 0;

        public override string Describe()
        {
            string name = goal != null && goal.targetEnemy != null ? goal.targetEnemy.name : "<unset enemy>";
            return $"Slay {name}: {currentCount} / {RequiredCount}";
        }

        public override bool OnEnemyDied(EnemyDiedEvent evt)
        {
            if (goal == null || goal.targetEnemy == null) return false;
            if (evt.Source != goal.targetEnemy) return false;
            if (currentCount >= goal.requiredCount) return false; // already done — don't over-count
            currentCount++;
            return true;
        }
    }
}
