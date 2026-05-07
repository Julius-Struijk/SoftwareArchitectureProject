using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using CMGTSA.Battle;
using CMGTSA.Boss;
using CMGTSA.Core;
using CMGTSA.Enemies;
using CMGTSA.Player;

namespace CMGTSA.Tests
{
    /// <summary>
    /// Verifies the boss-projectile -> EnemyAttackRequestedEvent ->
    /// DamageResolver -> player damage path. Exercises the bug fixed
    /// in section 2.3 of the post-slice-8 polish spec: the published
    /// event range used to be 0.1f which routinely missed the player's
    /// collider on overlap.
    /// </summary>
    public class BossProjectileDamageTests
    {
        private const int PlayerLayer = 6;
        private const int EnemyLayer  = 7;

        [UnitySetUp]
        public IEnumerator UnitySetUp()
        {
            EventBus<EnemyAttackRequestedEvent>.Clear();
            EventBus<DamageDealtEvent>.Clear();
            EventBus<PlayerHPChangedEvent>.Clear();
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator UnityTearDown()
        {
            EventBus<EnemyAttackRequestedEvent>.Clear();
            EventBus<DamageDealtEvent>.Clear();
            EventBus<PlayerHPChangedEvent>.Clear();
            yield return null;
        }

        [UnityTest]
        public IEnumerator BossProjectile_overlap_publishes_attack_event_with_radius_that_hits_player()
        {
            // ---- Arrange resolver ----
            var resolverHost = new GameObject("Resolver");
            var resolver = resolverHost.AddComponent<DamageResolver>();
            // The masks are private SerializeFields. Set to "everything" so the
            // overlap circle returns our test player regardless of layer.
            SetPrivateField(resolver, "playerMask", (LayerMask)(-1));
            SetPrivateField(resolver, "enemyMask",  (LayerMask)(-1));

            // ---- Arrange a synthetic IDamageable target standing in for the player ----
            var target = new GameObject("PlayerStand-in") { layer = PlayerLayer };
            target.transform.position = new Vector3(0f, 0f, 0f);
            target.AddComponent<CircleCollider2D>().radius = 0.5f;
            var probe = target.AddComponent<DamageableProbe>();

            // ---- Arrange projectile ----
            var projectileGO = new GameObject("BossProjectile");
            projectileGO.transform.position = new Vector3(0.4f, 0f, 0f); // adjacent to player
            projectileGO.AddComponent<CircleCollider2D>().isTrigger = true;
            var projectile = projectileGO.AddComponent<BossProjectile>();
            // Configure() needs a DamageData; speed and direction don't matter
            // because OnTriggerEnter2D is what we're exercising.
            projectile.Configure(Vector2.right, 0f, new DamageData { damage = 3 });
            // The playerLayer mask on BossProjectile filters trigger callbacks.
            // Set it to "everything" so the trigger fires on our stand-in regardless of layer.
            SetPrivateField(projectile, "playerLayer", (LayerMask)(-1));

            // ---- Act: simulate the trigger ----
            // Manually invoke the private OnTriggerEnter2D via reflection so the
            // test does not depend on Unity's physics tick to fire the callback.
            var method = typeof(BossProjectile).GetMethod(
                "OnTriggerEnter2D",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            Assert.IsNotNull(method, "BossProjectile.OnTriggerEnter2D not found");
            method.Invoke(projectile, new object[] { target.GetComponent<CircleCollider2D>() });

            yield return null;

            // ---- Assert ----
            Assert.AreEqual(1, probe.HitCount,
                "DamageableProbe should have received one TakeDamage call. " +
                "If 0, the published event range is still too small to overlap the player collider.");
            Assert.AreEqual(3, probe.LastAmount, "Damage amount should match the configured DamageData.");

            // ---- Cleanup ----
            Object.DestroyImmediate(resolverHost);
            Object.DestroyImmediate(target);
            // Projectile destroys itself in OnTriggerEnter2D; if it survived, kill it.
            if (projectileGO != null) Object.DestroyImmediate(projectileGO);
        }

        private static void SetPrivateField(object target, string name, object value)
        {
            var f = target.GetType().GetField(name,
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            Assert.IsNotNull(f, $"Field {name} not found on {target.GetType().Name}");
            f.SetValue(target, value);
        }

        private class DamageableProbe : MonoBehaviour, IDamageable
        {
            public int HitCount;
            public int LastAmount;

            public bool TakeDamage(int amount)
            {
                HitCount++;
                LastAmount = amount;
                return false;
            }
        }
    }
}
