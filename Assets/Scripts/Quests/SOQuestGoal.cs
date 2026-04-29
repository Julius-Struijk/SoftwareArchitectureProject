using UnityEngine;

namespace CMGTSA.Quests
{
    /// <summary>
    /// Abstract base for polymorphic quest goals. Each concrete subclass is its own
    /// ScriptableObject type with its own [CreateAssetMenu]. The runtime counter lives on
    /// a paired <see cref="IQuestGoalRuntime"/> instance produced by <see cref="CreateRuntime"/>
    /// — the SO itself is shared, immutable data; runtimes are per-quest-instance state.
    /// </summary>
    public abstract class SOQuestGoal : ScriptableObject
    {
        /// <summary>Player-facing one-line description (e.g. "Slay 2 Ghosts").</summary>
        public abstract string Describe();

        /// <summary>Factory: produce a fresh runtime tracker for one quest assignment.</summary>
        public abstract IQuestGoalRuntime CreateRuntime();
    }
}
