using UnityEngine;
using CMGTSA.Enemies;

namespace CMGTSA.Quests
{
    /// <summary>
    /// Concrete <see cref="SOQuestGoal"/>: completes when <see cref="requiredCount"/> enemies
    /// of type <see cref="targetEnemy"/> have died. Identity is the <c>EnemyData</c> SO
    /// reference (matches how <c>EnemyDiedEvent.Source</c> is filled by <c>EnemyController</c>).
    /// </summary>
    [CreateAssetMenu(fileName = "KillEnemyGoal", menuName = "Scriptable Objects/Quests/KillEnemyGoal")]
    public class KillEnemyGoal : SOQuestGoal
    {
        [Tooltip("EnemyData SO whose deaths count toward this goal. Compared by reference equality.")]
        public EnemyData targetEnemy;

        [Tooltip("How many of targetEnemy must die for this goal to complete.")]
        [Min(1)] public int requiredCount = 1;

        public override string Describe()
        {
            string name = targetEnemy != null ? targetEnemy.name : "<unset enemy>";
            return $"Slay {requiredCount} {name}";
        }

        public override IQuestGoalRuntime CreateRuntime()
        {
            return new KillEnemyGoalRuntime(this);
        }
    }
}
