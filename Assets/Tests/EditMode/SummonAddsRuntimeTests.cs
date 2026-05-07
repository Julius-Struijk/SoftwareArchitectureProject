using System;
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
            pattern.maxAttempts = 1; // one attempt; FakeBossContext accepts all positions by default
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

        [Test]
        public void Tick_SkipsAddWhenAllAttemptsRejectedByNavMesh()
        {
            var ctx = new FakeBossContext();
            ctx.BossPositionField = Vector3.zero;
            ctx.NavMeshPredicate = _ => false;
            pattern.maxAttempts = 3;

            var rt = pattern.CreateRuntime();
            rt.OnBegin(ctx);
            rt.Tick(0f, ctx);

            Assert.AreEqual(0, ctx.AddCalls.Count, "No add should spawn when sampling never succeeds.");
            Assert.AreEqual(pattern.addCount * 3, ctx.NavMeshSampleCalls.Count,
                "Spawner should have tried maxAttempts samples per add.");
            Assert.IsTrue(rt.IsFinished, "Runtime should still mark itself finished after failing.");
        }

        [Test]
        public void Tick_PassesSampleDistanceToContext()
        {
            var ctx = new FakeBossContext();
            ctx.BossPositionField = Vector3.zero;
            pattern.sampleDistance = 1.7f;

            var rt = pattern.CreateRuntime();
            rt.OnBegin(ctx);
            rt.Tick(0f, ctx);

            Assert.AreEqual(pattern.addCount, ctx.NavMeshSampleCalls.Count,
                "One sample per add when first attempt accepts.");
            for (int i = 0; i < ctx.NavMeshSampleCalls.Count; i++)
            {
                Assert.AreEqual(1.7f, ctx.NavMeshSampleCalls[i].MaxDistance, 0.0001f,
                    $"Sample {i} should pass pattern.sampleDistance through.");
            }
        }

        [Test]
        public void Tick_OnlySpawnsAtNavMeshAcceptedPositions()
        {
            var ctx = new FakeBossContext();
            ctx.BossPositionField = Vector3.zero;
            ctx.NavMeshPredicate = pos => pos.x <= 0f;
            pattern.maxAttempts = 6;

            var rt = pattern.CreateRuntime();
            rt.OnBegin(ctx);
            rt.Tick(0f, ctx);

            for (int i = 0; i < ctx.AddCalls.Count; i++)
            {
                Assert.LessOrEqual(ctx.AddCalls[i].Position.x, 0f,
                    $"Add {i} spawned at x>0 — that side was 'wall'.");
            }
        }
    }
}
