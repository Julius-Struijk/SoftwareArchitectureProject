using NUnit.Framework;
using UnityEngine;
using CMGTSA.Battle;
using CMGTSA.Boss;

namespace CMGTSA.Tests.EditMode
{
    public class ProjectileSprayRuntimeTests
    {
        private ProjectileSprayPattern pattern;
        private GameObject dummyPrefab;

        [SetUp]
        public void SetUp()
        {
            dummyPrefab = new GameObject("ProjectileDummy");
            pattern = ScriptableObject.CreateInstance<ProjectileSprayPattern>();
            pattern.cooldownSeconds = 3f;
            pattern.range = 6f;
            pattern.projectilePrefab = dummyPrefab;
            pattern.projectileCount = 3;
            pattern.spreadDegrees = 30f;
            pattern.projectileSpeed = 5f;
            pattern.intervalSeconds = 0.2f;
            pattern.damage = new DamageData { damage = 2 };
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(pattern);
            Object.DestroyImmediate(dummyPrefab);
        }

        [Test]
        public void Tick_FiresFirstProjectileImmediately()
        {
            var ctx = new FakeBossContext();
            var rt = pattern.CreateRuntime();
            rt.OnBegin(ctx);

            rt.Tick(0f, ctx);

            Assert.AreEqual(1, ctx.ProjectileCalls.Count);
            Assert.IsFalse(rt.IsFinished);
        }

        [Test]
        public void Tick_PacesProjectilesOverInterval()
        {
            var ctx = new FakeBossContext();
            var rt = pattern.CreateRuntime();
            rt.OnBegin(ctx);

            rt.Tick(0f, ctx);  // fire 1 immediately
            rt.Tick(0.1f, ctx); // not yet
            Assert.AreEqual(1, ctx.ProjectileCalls.Count);

            rt.Tick(0.1f, ctx); // fire 2 at t=0.2
            Assert.AreEqual(2, ctx.ProjectileCalls.Count);

            rt.Tick(0.2f, ctx); // fire 3 at t=0.4
            Assert.AreEqual(3, ctx.ProjectileCalls.Count);
            Assert.IsTrue(rt.IsFinished);
        }

        [Test]
        public void Tick_FansDirectionsAcrossSpread()
        {
            var ctx = new FakeBossContext();
            ctx.BossPositionField = Vector3.zero;
            ctx.PlayerPositionField = new Vector3(10f, 0f, 0f); // straight right
            var rt = pattern.CreateRuntime();
            rt.OnBegin(ctx);

            rt.Tick(0f, ctx);
            rt.Tick(0.2f, ctx);
            rt.Tick(0.2f, ctx);

            Assert.AreEqual(3, ctx.ProjectileCalls.Count);
            float a0 = Vector2.SignedAngle(Vector2.right, ctx.ProjectileCalls[0].Direction);
            float a1 = Vector2.SignedAngle(Vector2.right, ctx.ProjectileCalls[1].Direction);
            float a2 = Vector2.SignedAngle(Vector2.right, ctx.ProjectileCalls[2].Direction);
            Assert.AreEqual(-15f, a0, 1f);
            Assert.AreEqual(0f, a1, 1f);
            Assert.AreEqual(15f, a2, 1f);
        }

        [Test]
        public void Tick_AfterFinished_DoesNotFireMore()
        {
            var ctx = new FakeBossContext();
            var rt = pattern.CreateRuntime();
            rt.OnBegin(ctx);
            rt.Tick(0f, ctx);
            rt.Tick(0.2f, ctx);
            rt.Tick(0.2f, ctx);
            rt.Tick(0.2f, ctx);
            rt.Tick(0.2f, ctx);

            Assert.AreEqual(3, ctx.ProjectileCalls.Count);
        }
    }
}
