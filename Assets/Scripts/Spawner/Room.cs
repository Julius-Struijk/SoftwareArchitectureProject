using System.Collections.Generic;
using UnityEngine;
using CMGTSA.Enemies;

namespace CMGTSA.Spawner
{
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

        private void Awake()
        {
            var col = GetComponent<BoxCollider2D>();
            if (col != null)
            {
                Bounds = col.bounds;
            }
            else
            {
                Bounds = new Bounds(transform.position, Vector3.zero);
            }
        }

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
            var col = GetComponent<BoxCollider2D>();
            Gizmos.color = new Color(0f, 1f, 0f, 0.6f);
            if (col != null)
            {
                Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
            }
        }
    }
}
