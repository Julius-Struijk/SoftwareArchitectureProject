using UnityEngine;

namespace CMGTSA.Spawner
{
    /// <summary>
    /// Stamped onto an enemy GameObject by <see cref="EnemySpawner"/> at spawn time. Holds
    /// the back-reference to the <see cref="Room"/> that owns this enemy's life-counter, so
    /// the spawner can decrement that room's <c>AliveCount</c> on <c>EnemyDiedEvent</c>.
    /// Hand-placed enemies without this component are intentionally ignored by the spawner.
    /// Set is public because EnemySpawner assigns it immediately after AddComponent.
    /// </summary>
    public class EnemyRoomTag : MonoBehaviour
    {
        public Room Room { get; set; }
    }
}
