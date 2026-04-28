using System;
using UnityEngine;
using CMGTSA.Core;
using CMGTSA.Enemies;

namespace CMGTSA.Inventory
{
    /// <summary>
    /// MonoBehaviour glue: subscribes to <see cref="EnemyDiedEvent"/> and runs the testable
    /// <see cref="PureCore"/> on each death. Lives on the <c>GameSystems</c> scene object.
    /// </summary>
    public class LootDropper : MonoBehaviour
    {
        private PureCore core;

        private void Awake()
        {
            core = new PureCore(() => UnityEngine.Random.value);
        }

        private void OnEnable()
        {
            EventBus<EnemyDiedEvent>.Subscribe(OnEnemyDied);
        }

        private void OnDisable()
        {
            EventBus<EnemyDiedEvent>.Unsubscribe(OnEnemyDied);
        }

        private void OnEnemyDied(EnemyDiedEvent evt)
        {
            core.RollAndPublish(evt.Source, evt.Position);
        }

        /// <summary>
        /// Pure rolling logic. Public + nested so EditMode tests can inject a deterministic
        /// roll source without spinning up Unity's RNG.
        /// </summary>
        public class PureCore
        {
            private readonly Func<float> rollSource;
            public PureCore(Func<float> rollSource) { this.rollSource = rollSource; }

            public void RollAndPublish(EnemyData source, Vector3 position)
            {
                if (source == null || source.lootTable == null) return;
                for (int i = 0; i < source.lootTable.Length; i++)
                {
                    var entry = source.lootTable[i];
                    if (entry == null || entry.item == null) continue;
                    if (rollSource() >= entry.chance) continue;
                    EventBus<ItemDroppedEvent>.Publish(new ItemDroppedEvent(entry.item, position));
                }
            }
        }
    }
}
