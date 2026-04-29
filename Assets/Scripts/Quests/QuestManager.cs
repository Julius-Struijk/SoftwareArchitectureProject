using System.Collections.Generic;
using UnityEngine;
using CMGTSA.Core;
using CMGTSA.Enemies;
using CMGTSA.Game;
using CMGTSA.Inventory;

namespace CMGTSA.Quests
{
    /// <summary>
    /// MonoBehaviour glue: subscribes to <see cref="GameRestartedEvent"/>,
    /// <see cref="EnemyDiedEvent"/>, and <see cref="ItemPickedUpEvent"/> and routes each into
    /// the testable nested <see cref="PureCore"/>. The core holds the active-quest list and
    /// publishes <see cref="QuestStartedEvent"/>, <see cref="QuestProgressEvent"/>,
    /// <see cref="QuestCompletedEvent"/> on the bus.
    /// </summary>
    public class QuestManager : MonoBehaviour
    {
        [Header("Starter quests")]
        [Tooltip("Auto-assigned at GameRestartedEvent. Order matters: cards render in this order.")]
        [SerializeField] private QuestData[] startingQuests;

        private PureCore core;

        private void Awake()
        {
            core = new PureCore();
        }

        private void OnEnable()
        {
            EventBus<GameRestartedEvent>.Subscribe(OnGameRestarted);
            EventBus<EnemyDiedEvent>.Subscribe(OnEnemyDied);
            EventBus<ItemPickedUpEvent>.Subscribe(OnItemPickedUp);
        }

        private void OnDisable()
        {
            EventBus<GameRestartedEvent>.Unsubscribe(OnGameRestarted);
            EventBus<EnemyDiedEvent>.Unsubscribe(OnEnemyDied);
            EventBus<ItemPickedUpEvent>.Unsubscribe(OnItemPickedUp);
        }

        private void OnGameRestarted(GameRestartedEvent _)
        {
            core.AssignStartingQuests(startingQuests);
        }

        private void OnEnemyDied(EnemyDiedEvent evt)
        {
            core.HandleEnemyDied(evt);
        }

        private void OnItemPickedUp(ItemPickedUpEvent evt)
        {
            core.HandleItemPickedUp(evt);
        }

        /// <summary>
        /// All quest tracking logic. Public + nested so EditMode tests can exercise it without
        /// instantiating the MonoBehaviour or the EventBus subscription dance.
        /// </summary>
        public class PureCore
        {
            private readonly List<QuestProgress> active = new List<QuestProgress>();

            public IReadOnlyList<QuestProgress> Active => active;

            public void AssignStartingQuests(QuestData[] quests)
            {
                active.Clear();
                if (quests == null) return;
                for (int i = 0; i < quests.Length; i++)
                {
                    if (quests[i] == null) continue;
                    var progress = quests[i].CreateProgress();
                    active.Add(progress);
                    EventBus<QuestStartedEvent>.Publish(new QuestStartedEvent(quests[i], progress));
                }
            }

            public void HandleEnemyDied(EnemyDiedEvent evt)
            {
                Dispatch((g, e) => g.OnEnemyDied(e), evt);
            }

            public void HandleItemPickedUp(ItemPickedUpEvent evt)
            {
                Dispatch((g, e) => g.OnItemPickedUp(e), evt);
            }

            // Generic dispatcher: invokes hook on each goal of each active quest, publishes
            // Progress for any quest whose state changed, then publishes Completed exactly once
            // per quest the first time IsComplete flips true.
            private void Dispatch<T>(System.Func<IQuestGoalRuntime, T, bool> hook, T evt)
            {
                for (int q = 0; q < active.Count; q++)
                {
                    var quest = active[q];
                    if (quest == null) continue;
                    bool anyChanged = false;
                    for (int g = 0; g < quest.Goals.Length; g++)
                    {
                        var goal = quest.Goals[g];
                        if (goal == null) continue;
                        if (hook(goal, evt)) anyChanged = true;
                    }
                    if (!anyChanged) continue;

                    EventBus<QuestProgressEvent>.Publish(new QuestProgressEvent(quest.Data, quest));

                    if (quest.IsComplete && !quest.CompletionPublished)
                    {
                        quest.CompletionPublished = true;
                        int xp = quest.Data != null ? quest.Data.xpReward : 0;
                        EventBus<QuestCompletedEvent>.Publish(new QuestCompletedEvent(quest.Data, xp));
                    }
                }
            }
        }
    }
}
