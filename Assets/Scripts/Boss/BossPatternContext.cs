using UnityEngine;
using CMGTSA.Battle;
using CMGTSA.Core;
using CMGTSA.Enemies;

namespace CMGTSA.Boss
{
    public class BossPatternContext : IBossContext
    {
        private readonly Transform bossTransform;
        private readonly Transform playerTransform;

        public BossPatternContext(Transform boss, Transform player)
        {
            bossTransform = boss;
            playerTransform = player;
        }

        public Vector3 BossPosition   => bossTransform   != null ? bossTransform.position   : Vector3.zero;
        public Vector3 PlayerPosition => playerTransform != null ? playerTransform.position : Vector3.zero;

        public void RequestMeleeAttack(float range, DamageData damage)
        {
            if (bossTransform == null || damage == null) return;
            EventBus<EnemyAttackRequestedEvent>.Publish(new EnemyAttackRequestedEvent(
                bossTransform.position, range, damage, bossTransform));
        }

        public void SpawnProjectile(GameObject prefab, Vector3 origin, Vector2 direction, float speed, DamageData damage)
        {
            if (prefab == null) return;
            GameObject instance = Object.Instantiate(prefab, origin, Quaternion.identity);
            BossProjectile projectile = instance.GetComponent<BossProjectile>();
            if (projectile != null) projectile.Configure(direction, speed, damage);
        }

        public void SpawnAdd(GameObject prefab, Vector3 position)
        {
            if (prefab == null) return;
            Object.Instantiate(prefab, position, Quaternion.identity);
        }
    }
}
