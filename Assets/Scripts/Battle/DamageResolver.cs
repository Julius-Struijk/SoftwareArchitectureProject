using UnityEngine;
using CMGTSA.Core;
using CMGTSA.Player;
using CMGTSA.Enemies;

namespace CMGTSA.Battle
{
    /// <summary>
    /// Central damage dispatcher. Subscribes to <see cref="PlayerAttackRequestedEvent"/> and
    /// <see cref="EnemyAttackRequestedEvent"/>, casts an OverlapCircle against the relevant
    /// LayerMask, calls <c>IDamageable.TakeDamage</c> on each hit, and announces each
    /// confirmed hit on the bus via <see cref="DamageDealtEvent"/>.
    ///
    /// Lives on the GameSystems GameObject in the scene. Exactly one instance per scene.
    /// </summary>
    public class DamageResolver : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Layers a player swing can hit. Set to the Enemy layer.")]
        private LayerMask enemyMask;

        [SerializeField]
        [Tooltip("Layers an enemy swing can hit. Set to the Player layer.")]
        private LayerMask playerMask;

        private void OnEnable()
        {
            EventBus<PlayerAttackRequestedEvent>.Subscribe(OnPlayerAttack);
            EventBus<EnemyAttackRequestedEvent>.Subscribe(OnEnemyAttack);
        }

        private void OnDisable()
        {
            EventBus<PlayerAttackRequestedEvent>.Unsubscribe(OnPlayerAttack);
            EventBus<EnemyAttackRequestedEvent>.Unsubscribe(OnEnemyAttack);
        }

        private void OnPlayerAttack(PlayerAttackRequestedEvent evt)
        {
            // Sweep a circle in front of the player along the facing direction.
            Vector3 dir = evt.Direction.sqrMagnitude > 0.0001f
                ? (Vector3)evt.Direction.normalized
                : Vector3.right;
            Vector3 center = evt.Origin + dir * (evt.Range * 0.5f);

            ResolveHits(center, evt.Range, enemyMask, evt.Damage);
        }

        private void OnEnemyAttack(EnemyAttackRequestedEvent evt)
        {
            // Enemy attack: circle is centred on the enemy itself (slice-2 melee).
            ResolveHits(evt.Origin, evt.Range, playerMask, evt.Damage);
        }

        private void ResolveHits(Vector3 center, float radius, LayerMask mask, DamageData damage)
        {
            if (damage == null) return;

            Collider2D[] hits = Physics2D.OverlapCircleAll(center, radius, mask);
            for (int i = 0; i < hits.Length; i++)
            {
                IDamageable target = hits[i].GetComponentInParent<IDamageable>();
                if (target == null) continue;

                bool killed = target.TakeDamage(damage.damage);
                EventBus<DamageDealtEvent>.Publish(new DamageDealtEvent(
                    hits[i].transform.position,
                    damage.damage,
                    damage,
                    hits[i].transform,
                    killed));
            }
        }
    }
}
