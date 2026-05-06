using NUnit.Framework;
using UnityEngine;
using CMGTSA.Boss;

namespace CMGTSA.Tests.EditMode
{
    public class SummonAddsRuntimeTests
    {
        private SummonAddsPattern pattern;
        private GameObject dummyPrefab;

        [SetUp]
        public void SetUp()
        {
            dummyPrefab = new GameObject("AddDummy");
            pattern = ScriptableObject.CreateInstance<SummonAddsPattern>();
            pattern.cooldownSeconds = 5f;
            pattern.range = 0f;
            pattern.addPrefab = dummyPrefab;
            pattern.addCount = 4;
            pattern.spawnRadius = 2f;
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(pattern);
            Object.DestroyImmediate(dummyPrefab);
        }

        [Test]
        public void Tick_SpawnsAllAddsImmediately()
        {
            var ctx = new FakeBossContext();
            ctx.BossPositionField = new Vector3(5f, 5f, 0f);
            var rt = pattern.CreateRuntime();
            rt.OnBegin(ctx);

            rt.Tick(0f, ctx);

            Assert.AreEqual(4, ctx.AddCalls.Count);
            Assert.IsTrue(rt.IsFinished);
        }

        [Test]
        public void Tick_PlacesAddsOnRingAroundBoss()
        {
            var ctx = new FakeBossContext();
            ctx.BossPositionField = Vector3.zero;
            var rt = pattern.CreateRuntime();
            rt.OnBegin(ctx);

            rt.Tick(0f, ctx);

            for (int i = 0; i < ctx.AddCalls.Count; i++)
            {
                float distance = Vector3.Distance(Vector3.zero, ctx.AddCalls[i].Position);
                Assert.AreEqual(2f, distance, 0.001f, $"Add {i} not on ring");
            }
        }

        [Test]
        public void Tick_AfterFinished_DoesNotSpawnMore()
        {
            var ctx = new FakeBossContext();
            var rt = pattern.CreateRuntime();
            rt.OnBegin(ctx);
            rt.Tick(0f, ctx);
            rt.Tick(0.1f, ctx);
            rt.Tick(0.1f, ctx);

            Assert.AreEqual(4, ctx.AddCalls.Count);
        }
    }
}
