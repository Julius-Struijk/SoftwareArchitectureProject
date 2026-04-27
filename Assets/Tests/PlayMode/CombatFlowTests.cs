using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using CMGTSA.Battle;
using CMGTSA.Core;
using CMGTSA.Enemies;
using CMGTSA.Player;

namespace CMGTSA.Tests
{
    /// <summary>
    /// End-to-end PlayMode coverage of the slice-2 damage flow:
    /// PlayerAttackRequestedEvent -> DamageResolver overlap -> IDamageable.TakeDamage
    /// -> EnemyController HP drop -> EnemyDiedEvent -> XP grant.
    /// </summary>
    public class CombatFlowTests
    {
        private const int PlayerLayer = 6;
        private const int EnemyLayer  = 7;

        [UnitySetUp]
        public IEnumerator UnitySetUp()
        {
            EventBus<PlayerAttackRequestedEvent>.Clear();
            EventBus<EnemyAttackRequestedEvent>.Clear();
            EventBus<DamageDealtEvent>.Clear();
            EventBus<EnemyDiedEvent>.Clear();
            EventBus<PlayerHPChangedEvent>.Clear();
            EventBus<PlayerXPGainedEvent>.Clear();
            EventBus<PlayerLeveledUpEvent>.Clear();
            EventBus<PlayerDiedEvent>.Clear();

            yield return SceneManager.LoadSceneAsync(
                "CombatPlayModeScene", LoadSceneMode.Single);
        }

        [UnityTearDown]
        public IEnumerator UnityTearDown()
        {
            // Scene is reloaded on the next test's UnitySetUp. Clean up bus to be safe.
            EventBus<PlayerAttackRequestedEvent>.Clear();
            EventBus<EnemyAttackRequestedEvent>.Clear();
            EventBus<DamageDealtEvent>.Clear();
            EventBus<EnemyDiedEvent>.Clear();
            EventBus<PlayerHPChangedEvent>.Clear();
            EventBus<PlayerXPGainedEvent>.Clear();
            EventBus<PlayerLeveledUpEvent>.Clear();
            EventBus<PlayerDiedEvent>.Clear();
            yield return null;
        }

        [UnityTest]
        public IEnumerator PlayerAttack_event_damages_enemy_in_range()
        {
            var resolverHost = new GameObject("Resolver");
            var resolver = resolverHost.AddComponent<DamageResolver>();
            // Reflection: the masks are private SerializeFields. Easier: build a stand-alone
            // resolver by setting masks via SerializedObject in Editor — but here we use
            // Physics2D.queriesHitTriggers = true and unrestricted layer masks so OverlapCircle
            // returns everything we need. The masks default to 0 from Unity's serialiser; set
            // to -1 (everything) via reflection to mirror designer-set behaviour.
            SetPrivateMask(resolver, "enemyMask", -1);
            SetPrivateMask(resolver, "playerMask", -1);

            // Build a synthetic enemy: a GameObject with Collider2D + EnemyController + an
            // EnemyData ScriptableObject created on the fly.
            var enemyData = ScriptableObject.CreateInstance<EnemyData>();
            enemyData.maxHP = 4;
            enemyData.xp = 7;
            enemyData.money = 0;
            enemyData.attackInterval = 0.5f;
            enemyData.distanceThreshold = 0.2f;
            enemyData.normalModeSpeed = 0f;
            enemyData.alertModeSpeed = 0f;
            enemyData.normalModeWaitingTime = 999f;
            enemyData.alertModeWaitingTime = 999f;
            enemyData.attackRange = 1f;
            enemyData.chaseRange = 1f;
            enemyData.attackDamage = new DamageData { damage = 1 };

            // Player tag is required because EnemyController.Awake calls FindGameObjectWithTag.
            var dummyPlayer = new GameObject("PlayerDummy") { tag = "Player" };
            dummyPlayer.transform.position = new Vector3(0, 0, 0);

            var enemyGO = new GameObject("Enemy");
            enemyGO.layer = EnemyLayer;
            enemyGO.transform.position = new Vector3(2, 0, 0);
            enemyGO.AddComponent<CircleCollider2D>().radius = 0.5f;
            // NavMeshAgent is required by EnemyController; add but disable navmesh.
            var nav = enemyGO.AddComponent<UnityEngine.AI.NavMeshAgent>();
            nav.enabled = false;
            var enemyCtrl = enemyGO.AddComponent<EnemyController>();
            SetPrivateField(enemyCtrl, "enemyData", enemyData);
            SetPrivateField(enemyCtrl, "navMeshAgent", nav);

            yield return null;             // Awake runs
            yield return null;             // Start runs

            int xpGained = 0;
            EventBus<EnemyDiedEvent>.Subscribe(e => xpGained += e.XP);

            // Fire 4 attack events to chip away maxHP=4 with damage=1 each.
            var dmg = new DamageData { damage = 1 };
            for (int i = 0; i < 4; i++)
            {
                EventBus<PlayerAttackRequestedEvent>.Publish(new PlayerAttackRequestedEvent(
                    Vector3.zero, Vector2.right, 5f, dmg));
                yield return null;
            }

            Assert.AreEqual(7, xpGained, "EnemyDiedEvent fires once with the SO's XP value");
            Assert.IsTrue(enemyGO == null || !enemyGO.activeInHierarchy,
                "Enemy GameObject is destroyed on death");

            Object.DestroyImmediate(enemyData);
        }

        private static void SetPrivateField(object target, string name, object value)
        {
            var f = target.GetType().GetField(name,
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            Assert.IsNotNull(f, $"Field {name} not found on {target.GetType().Name}");
            f.SetValue(target, value);
        }

        private static void SetPrivateMask(object target, string name, int maskValue)
        {
            var f = target.GetType().GetField(name,
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            Assert.IsNotNull(f, $"LayerMask field {name} not found on {target.GetType().Name}");
            f.SetValue(target, (LayerMask)maskValue);
        }
    }
}
