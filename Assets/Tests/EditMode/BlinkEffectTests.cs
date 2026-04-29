using NUnit.Framework;
using UnityEngine;
using CMGTSA.Skills;

namespace CMGTSA.Tests
{
    public class BlinkEffectTests
    {
        private BlinkEffect effect;

        [SetUp]
        public void SetUp()
        {
            effect = ScriptableObject.CreateInstance<BlinkEffect>();
            effect.distance = 5f;
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(effect);
        }

        [Test]
        public void Activate_teleports_to_position_plus_facing_times_distance()
        {
            var ctx = new FakeSkillContext
            {
                PlayerPosition = new Vector3(2f, 3f, 0f),
                PlayerFacing = new Vector2(1f, 0f)
            };

            effect.Activate(ctx);

            Assert.AreEqual(1, ctx.TeleportCalls.Count);
            Assert.AreEqual(new Vector3(7f, 3f, 0f), ctx.TeleportCalls[0]);
        }

        [Test]
        public void Activate_normalizes_diagonal_facing_before_scaling()
        {
            var ctx = new FakeSkillContext
            {
                PlayerPosition = Vector3.zero,
                PlayerFacing = new Vector2(3f, 4f) // magnitude 5 -> normalized (0.6, 0.8)
            };

            effect.Activate(ctx);

            Vector3 expected = new Vector3(0.6f * 5f, 0.8f * 5f, 0f);
            Assert.AreEqual(1, ctx.TeleportCalls.Count);
            Assert.That(ctx.TeleportCalls[0].x, Is.EqualTo(expected.x).Within(0.0001f));
            Assert.That(ctx.TeleportCalls[0].y, Is.EqualTo(expected.y).Within(0.0001f));
        }

        [Test]
        public void Activate_with_zero_facing_falls_back_to_right()
        {
            var ctx = new FakeSkillContext
            {
                PlayerPosition = Vector3.zero,
                PlayerFacing = Vector2.zero
            };

            effect.Activate(ctx);

            Assert.AreEqual(1, ctx.TeleportCalls.Count);
            Assert.AreEqual(new Vector3(5f, 0f, 0f), ctx.TeleportCalls[0]);
        }

        [Test]
        public void OnLearned_is_a_noop()
        {
            var ctx = new FakeSkillContext();
            effect.OnLearned(ctx);
            Assert.AreEqual(0, ctx.TeleportCalls.Count);
        }

        [Test]
        public void Activate_with_null_context_does_not_throw()
        {
            Assert.DoesNotThrow(() => effect.Activate(null));
        }
    }
}
