using System.Collections.Generic;
using UnityEngine;
using CMGTSA.Enemies;

namespace CMGTSA.Spawner
{
    /// <summary>
    /// Defines a dungeon room for the spawner system. The sibling <see cref="BoxCollider2D"/>
    /// supplies both the trigger volume (used by <see cref="RoomTrigger"/>) and the spawn
    /// bounds (used by <see cref="EnemySpawner"/>). Counter mutators are public so the
    /// spawner can update <see cref="AliveCount"/> and <see cref="LastSpawnAt"/> without
    /// reflection — this is one project, encapsulation purity isn't worth the ceremony.
    /// </summary>
    public class Room : MonoBehaviour
    {
        [Tooltip("Pool of enemy SOs the spawner picks from for this room. Empty = no spawns.")]
        [SerializeField] private EnemyData[] enemyPool;

        [Tooltip("Max alive enemies spawned by this spawner in this room. Hand-placed enemies " +
                 "without an EnemyRoomTag do not count.")]
        [SerializeField] private int simultaneousCap = 3;

        [Tooltip("Seconds between consecutive spawns in this room.")]
        [SerializeField] private float respawnInterval = 4f;

        public IReadOnlyList<EnemyData> EnemyPool => enemyPool;
        public int SimultaneousCap => simultaneousCap;
        public float RespawnInterval => respawnInterval;
        public int AliveCount { get; private set; }
        public float LastSpawnAt { get; private set; }
        public Bounds Bounds { get; private set; }

        private BoxCollider2D col;

        private void Awake()
        {
            col = GetComponent<BoxCollider2D>();
            Bounds = col != null ? col.bounds : new Bounds(transform.position, Vector3.zero);
        }

        /// <summary>
        /// Test/runtime seam: replace the serialized config in-memory. Called from EditMode
        /// tests (no Inspector). Production code does not call this — the Inspector populates
        /// the same fields at scene load. The explicit <paramref name="bounds"/> param overrides
        /// the collider-derived Bounds from Awake; the two paths are mutually exclusive by design.
        /// </summary>
        public void Configure(EnemyData[] pool, int cap, float interval, Bounds bounds)
        {
            enemyPool = pool;
            simultaneousCap = cap;
            respawnInterval = interval;
            Bounds = bounds;
        }

        public void IncrementAliveCount() => AliveCount++;
        public void DecrementAliveCount() => AliveCount = Mathf.Max(0, AliveCount - 1);
        public void StampLastSpawnAt(float t) => LastSpawnAt = t;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0f, 1f, 0f, 0.6f);
            if (col != null)
            {
                Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
            }
        }
    }
}
