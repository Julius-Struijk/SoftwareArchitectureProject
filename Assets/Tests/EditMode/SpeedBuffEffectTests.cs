using NUnit.Framework;
using UnityEngine;
using CMGTSA.Skills;

namespace CMGTSA.Tests
{
    public class SpeedBuffEffectTests
    {
        private SpeedBuffEffect effect;

        [SetUp]
        public void SetUp()
        {
            effect = ScriptableObject.CreateInstance<SpeedBuffEffect>();
            effect.multiplier = 1.5f;
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(effect);
        }

        [Test]
        public void IsPassive_returns_true()
        {
            Assert.IsTrue(effect.IsPassive);
        }

        [Test]
        public void OnLearned_calls_ApplySpeedMultiplier_with_configured_value()
        {
            var ctx = new FakeSkillContext();
            effect.OnLearned(ctx);
            Assert.AreEqual(1, ctx.SpeedMultiplierCalls.Count);
            Assert.AreEqual(1.5f, ctx.SpeedMultiplierCalls[0]);
        }

        [Test]
        public void Activate_is_a_noop_for_passive_effect()
        {
            var ctx = new FakeSkillContext();
            effect.Activate(ctx);
            Assert.AreEqual(0, ctx.SpeedMultiplierCalls.Count);
            Assert.AreEqual(0, ctx.AreaDamageCalls.Count);
            Assert.AreEqual(0, ctx.TeleportCalls.Count);
        }

        [Test]
        public void OnLearned_with_null_context_does_not_throw()
        {
            Assert.DoesNotThrow(() => effect.OnLearned(null));
        }
    }
}
