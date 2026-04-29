using NUnit.Framework;
using UnityEngine;
using CMGTSA.Battle;
using CMGTSA.Skills;

namespace CMGTSA.Tests
{
    public class AOESlamEffectTests
    {
        private AOESlamEffect effect;

        [SetUp]
        public void SetUp()
        {
            effect = ScriptableObject.CreateInstance<AOESlamEffect>();
            effect.radius = 3f;
            effect.damage = new DamageData { damage = 5 };
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(effect);
        }

        [Test]
        public void IsPassive_returns_false()
        {
            Assert.IsFalse(effect.IsPassive);
        }

        [Test]
        public void Activate_calls_ApplyAreaDamage_at_player_position_with_configured_radius()
        {
            var ctx = new FakeSkillContext { PlayerPosition = new Vector3(7f, -2f, 0f) };

            effect.Activate(ctx);

            Assert.AreEqual(1, ctx.AreaDamageCalls.Count);
            Assert.AreEqual(new Vector3(7f, -2f, 0f), ctx.AreaDamageCalls[0].origin);
            Assert.AreEqual(3f, ctx.AreaDamageCalls[0].radius);
            Assert.AreSame(effect.damage, ctx.AreaDamageCalls[0].damage);
        }

        [Test]
        public void OnLearned_is_a_noop()
        {
            var ctx = new FakeSkillContext();
            effect.OnLearned(ctx);
            Assert.AreEqual(0, ctx.AreaDamageCalls.Count);
            Assert.AreEqual(0, ctx.SpeedMultiplierCalls.Count);
        }

        [Test]
        public void Activate_with_null_context_does_not_throw()
        {
            Assert.DoesNotThrow(() => effect.Activate(null));
        }

        [Test]
        public void Activate_with_null_damage_does_not_throw()
        {
            effect.damage = null;
            var ctx = new FakeSkillContext();
            Assert.DoesNotThrow(() => effect.Activate(ctx));
            Assert.AreEqual(0, ctx.AreaDamageCalls.Count);
        }
    }
}
