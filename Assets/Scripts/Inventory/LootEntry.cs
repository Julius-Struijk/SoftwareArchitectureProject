using System;
using UnityEngine;

namespace CMGTSA.Inventory
{
    /// <summary>
    /// One row in <c>EnemyData.lootTable</c>. Independent rolls (a 50% potion + 25% key
    /// can produce both, just one, or nothing). Designer-friendly: stays in the Inspector.
    /// </summary>
    [Serializable]
    public class LootEntry
    {
        [Tooltip("The item to drop on a successful roll.")]
        public ItemData item;

        [Range(0f, 1f)]
        [Tooltip("0.5 = drops on 50% of kills. Each entry is rolled independently.")]
        public float chance = 0.25f;
    }
}
