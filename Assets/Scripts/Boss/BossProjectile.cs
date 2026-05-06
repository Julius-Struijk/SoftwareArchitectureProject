using UnityEngine;
using CMGTSA.Battle;
using CMGTSA.Core;
using CMGTSA.Enemies;

namespace CMGTSA.Boss
{
    [RequireComponent(typeof(Collider2D))]
    public class BossProjectile : MonoBehaviour
    {
        [SerializeField] private float lifetimeSeconds = 4f;
        [SerializeField] private LayerMask playerLayer;

        private Vector2 direction = Vector2.right;
        private float speed = 5f;
        private DamageData damage;
        private float spawnTime;

        public void Configure(Vector2 dir, float spd, DamageData dmg)
        {
            direction = dir.sqrMagnitude > 0.0001f ? dir.normalized : Vector2.right;
            speed = spd;
            damage = dmg;
        }

        private void OnEnable()
        {
            spawnTime = Time.time;
        }

        private void Update()
        {
            transform.position += (Vector3)(direction * (speed * Time.deltaTime));
            if (Time.time - spawnTime >= lifetimeSeconds)
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (((1 << other.gameObject.layer) & playerLayer.value) == 0) return;
            if (damage != null)
            {
                EventBus<EnemyAttackRequestedEvent>.Publish(new EnemyAttackRequestedEvent(
                    transform.position, 0.1f, damage, transform));
            }
            Destroy(gameObject);
        }
    }
}
