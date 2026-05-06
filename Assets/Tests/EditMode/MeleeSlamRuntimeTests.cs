using NUnit.Framework;
using UnityEngine;
using CMGTSA.Battle;
using CMGTSA.Boss;

namespace CMGTSA.Tests.EditMode
{
    public class MeleeSlamRuntimeTests
    {
        private MeleeSlamPattern pattern;

        [SetUp]
        public void SetUp()
        {
            pattern = ScriptableObject.CreateInstance<MeleeSlamPattern>();
            pattern.cooldownSeconds = 2f;
            pattern.range = 1.5f;
            pattern.windupSeconds = 0.5f;
            pattern.damage = new DamageData { damage = 4 };
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(pattern);
        }

        [Test]
        public void Tick_BeforeWindupEnds_DoesNotSwing()
        {
            var ctx = new FakeBossContext();
            var rt = pattern.CreateRuntime();
            rt.OnBegin(ctx);

            rt.Tick(0.2f, ctx);

            Assert.AreEqual(0, ctx.MeleeCalls.Count);
            Assert.IsFalse(rt.IsFinished);
        }

        [Test]
        public void Tick_AfterWindup_SwingsOnce()
        {
            var ctx = new FakeBossContext();
            var rt = pattern.CreateRuntime();
            rt.OnBegin(ctx);

            rt.Tick(0.5f, ctx);

            Assert.AreEqual(1, ctx.MeleeCalls.Count);
            Assert.AreEqual(1.5f, ctx.MeleeCalls[0].Range, 0.0001f);
            Assert.AreEqual(4, ctx.MeleeCalls[0].Damage.damage);
            Assert.IsTrue(rt.IsFinished);
        }

        [Test]
        public void Tick_AfterFinished_DoesNotSwingAgain()
        {
            var ctx = new FakeBossContext();
            var rt = pattern.CreateRuntime();
            rt.OnBegin(ctx);

            rt.Tick(0.5f, ctx);
            rt.Tick(0.5f, ctx);
            rt.Tick(0.5f, ctx);

            Assert.AreEqual(1, ctx.MeleeCalls.Count);
        }
    }
}
