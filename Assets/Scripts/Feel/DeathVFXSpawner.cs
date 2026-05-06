using UnityEngine;
using CMGTSA.Boss;
using CMGTSA.Core;
using CMGTSA.Enemies;

namespace CMGTSA.Feel
{
    /// <summary>
    /// Slice-7 polish: death particles. Subscribes to <see cref="EnemyDiedEvent"/> (regular
    /// enemies) and <see cref="BossEncounterEndedEvent"/> with Victory==true (boss).
    /// </summary>
    public class DeathVFXSpawner : MonoBehaviour
    {
        [Tooltip("Particle prefab spawned on every enemy/boss death.")]
        [SerializeField] private GameObject enemyDeathPrefab;

        [Tooltip("Optional: dedicated bigger burst spawned on boss death only. Falls back to enemyDeathPrefab if null.")]
        [SerializeField] private GameObject bossDeathPrefab;

        private void OnEnable()
        {
            EventBus<EnemyDiedEvent>.Subscribe(OnEnemyDied);
            EventBus<BossEncounterEndedEvent>.Subscribe(OnBossEnded);
        }

        private void OnDisable()
        {
            EventBus<EnemyDiedEvent>.Unsubscribe(OnEnemyDied);
            EventBus<BossEncounterEndedEvent>.Unsubscribe(OnBossEnded);
        }

        private void OnEnemyDied(EnemyDiedEvent evt)
        {
            Spawn(enemyDeathPrefab, evt.Position);
        }

        private void OnBossEnded(BossEncounterEndedEvent evt)
        {
            if (!evt.Victory) return;
            Spawn(bossDeathPrefab != null ? bossDeathPrefab : null, Vector3.zero);
        }

        private static void Spawn(GameObject prefab, Vector3 position)
        {
            if (prefab == null) return;
            Instantiate(prefab, position, Quaternion.identity);
        }
    }
}
