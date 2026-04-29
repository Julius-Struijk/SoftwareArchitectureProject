using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using CMGTSA.Core;
using CMGTSA.Skills;

namespace CMGTSA.Tests
{
    public class SkillManagerTests
    {
        private SpeedBuffEffect speedEffect;
        private AOESlamEffect slamEffect;
        private BlinkEffect blinkEffect;
        private SkillData speedSkill;
        private SkillData slamSkill;
        private SkillData blinkSkill;

        private float fakeNow;

        [SetUp]
        public void SetUp()
        {
            EventBus<SkillLearnedEvent>.Clear();
            EventBus<SkillUsedEvent>.Clear();

            speedEffect = ScriptableObject.CreateInstance<SpeedBuffEffect>();
            speedEffect.multiplier = 1.2f;
            slamEffect = ScriptableObject.CreateInstance<AOESlamEffect>();
            slamEffect.radius = 2f;
            slamEffect.damage = new CMGTSA.Battle.DamageData { damage = 1 };
            blinkEffect = ScriptableObject.CreateInstance<BlinkEffect>();
            blinkEffect.distance = 3f;

            speedSkill = ScriptableObject.CreateInstance<SkillData>();
            speedSkill.displayName = "Speed";
            speedSkill.requiredLevel = 1;
            speedSkill.cooldownSeconds = 0f;
            speedSkill.effect = speedEffect;

            slamSkill = ScriptableObject.CreateInstance<SkillData>();
            slamSkill.displayName = "Slam";
            slamSkill.requiredLevel = 2;
            slamSkill.cooldownSeconds = 5f;
            slamSkill.effect = slamEffect;

            blinkSkill = ScriptableObject.CreateInstance<SkillData>();
            blinkSkill.displayName = "Blink";
            blinkSkill.requiredLevel = 3;
            blinkSkill.cooldownSeconds = 4f;
            blinkSkill.effect = blinkEffect;

            fakeNow = 0f;
        }

        [TearDown]
        public void TearDown()
        {
            EventBus<SkillLearnedEvent>.Clear();
            EventBus<SkillUsedEvent>.Clear();
            Object.DestroyImmediate(speedSkill);
            Object.DestroyImmediate(slamSkill);
            Object.DestroyImmediate(blinkSkill);
            Object.DestroyImmediate(speedEffect);
            Object.DestroyImmediate(slamEffect);
            Object.DestroyImmediate(blinkEffect);
        }

        private SkillManager.PureCore NewCore() => new SkillManager.PureCore(() => fakeNow);

        [Test]
        public void Reset_unlocks_skills_with_required_level_at_or_below_initial_level()
        {
            var core = NewCore();
            var ctx = new FakeSkillContext();
            var learned = new List<SkillLearnedEvent>();
            EventBus<SkillLearnedEvent>.Subscribe(learned.Add);

            core.Reset(new[] { speedSkill, slamSkill, blinkSkill }, ctx, initialLevel: 1);

            Assert.AreEqual(1, learned.Count);
            Assert.AreSame(speedSkill, learned[0].Skill);
            Assert.IsTrue(core.Runtimes[0].Unlocked);
            Assert.IsFalse(core.Runtimes[1].Unlocked);
            Assert.IsFalse(core.Runtimes[2].Unlocked);
        }

        [Test]
        public void Reset_invokes_OnLearned_on_unlocked_passive()
        {
            var core = NewCore();
            var ctx = new FakeSkillContext();

            core.Reset(new[] { speedSkill, slamSkill, blinkSkill }, ctx, initialLevel: 1);

            Assert.AreEqual(1, ctx.SpeedMultiplierCalls.Count);
            Assert.AreEqual(1.2f, ctx.SpeedMultiplierCalls[0]);
        }

        [Test]
        public void Active_slots_skip_passives_and_preserve_array_order()
        {
            var core = NewCore();
            var ctx = new FakeSkillContext();

            core.Reset(new[] { speedSkill, slamSkill, blinkSkill }, ctx, initialLevel: 1);

            Assert.AreEqual(3, core.Runtimes.Count);
            Assert.AreEqual(2, core.ActiveSlotCount);
        }

        [Test]
        public void HandleLevelUp_unlocks_newly_eligible_skills_only()
        {
            var core = NewCore();
            var ctx = new FakeSkillContext();
            core.Reset(new[] { speedSkill, slamSkill, blinkSkill }, ctx, initialLevel: 1);

            int learnedCount = 0;
            EventBus<SkillLearnedEvent>.Subscribe(_ => learnedCount++);

            core.HandleLevelUp(2, ctx);

            Assert.AreEqual(1, learnedCount);
            Assert.IsTrue(core.Runtimes[1].Unlocked);
            Assert.IsFalse(core.Runtimes[2].Unlocked);
        }

        [Test]
        public void HandleLevelUp_does_not_re_fire_for_already_unlocked_skill()
        {
            var core = NewCore();
            var ctx = new FakeSkillContext();
            core.Reset(new[] { speedSkill, slamSkill, blinkSkill }, ctx, initialLevel: 1);

            int learnedCount = 0;
            EventBus<SkillLearnedEvent>.Subscribe(_ => learnedCount++);

            core.HandleLevelUp(1, ctx);
            core.HandleLevelUp(2, ctx);
            core.HandleLevelUp(2, ctx);

            Assert.AreEqual(1, learnedCount);
        }

        [Test]
        public void HandleLevelUp_to_high_level_unlocks_all_remaining()
        {
            var core = NewCore();
            var ctx = new FakeSkillContext();
            core.Reset(new[] { speedSkill, slamSkill, blinkSkill }, ctx, initialLevel: 1);

            int learnedCount = 0;
            EventBus<SkillLearnedEvent>.Subscribe(_ => learnedCount++);

            core.HandleLevelUp(99, ctx);

            Assert.AreEqual(2, learnedCount);
            Assert.IsTrue(core.Runtimes[1].Unlocked);
            Assert.IsTrue(core.Runtimes[2].Unlocked);
        }

        [Test]
        public void HandleUseRequested_locked_slot_does_nothing()
        {
            var core = NewCore();
            var ctx = new FakeSkillContext();
            core.Reset(new[] { speedSkill, slamSkill, blinkSkill }, ctx, initialLevel: 1);
            int usedCount = 0;
            EventBus<SkillUsedEvent>.Subscribe(_ => usedCount++);

            core.HandleUseRequested(0, ctx); // slot 0 = slam, still locked

            Assert.AreEqual(0, usedCount);
            Assert.AreEqual(0, ctx.AreaDamageCalls.Count);
        }

        [Test]
        public void HandleUseRequested_unlocked_slot_fires_and_starts_cooldown()
        {
            var core = NewCore();
            var ctx = new FakeSkillContext();
            core.Reset(new[] { speedSkill, slamSkill, blinkSkill }, ctx, initialLevel: 2);
            SkillUsedEvent? used = null;
            EventBus<SkillUsedEvent>.Subscribe(e => used = e);

            fakeNow = 10f;
            core.HandleUseRequested(0, ctx); // slot 0 = slam, unlocked

            Assert.AreEqual(1, ctx.AreaDamageCalls.Count);
            Assert.IsTrue(used.HasValue);
            Assert.AreSame(slamSkill, used.Value.Skill);
            Assert.AreEqual(0, used.Value.SlotIndex);
            Assert.AreEqual(5f, used.Value.CooldownDuration);
        }

        [Test]
        public void Second_use_during_cooldown_is_blocked()
        {
            var core = NewCore();
            var ctx = new FakeSkillContext();
            core.Reset(new[] { speedSkill, slamSkill, blinkSkill }, ctx, initialLevel: 2);

            fakeNow = 0f;
            core.HandleUseRequested(0, ctx);
            fakeNow = 4.9f;
            core.HandleUseRequested(0, ctx);

            Assert.AreEqual(1, ctx.AreaDamageCalls.Count);
        }

        [Test]
        public void Second_use_after_cooldown_fires_again()
        {
            var core = NewCore();
            var ctx = new FakeSkillContext();
            core.Reset(new[] { speedSkill, slamSkill, blinkSkill }, ctx, initialLevel: 2);

            fakeNow = 0f;
            core.HandleUseRequested(0, ctx);
            fakeNow = 5.01f;
            core.HandleUseRequested(0, ctx);

            Assert.AreEqual(2, ctx.AreaDamageCalls.Count);
        }

        [Test]
        public void Out_of_range_slot_is_ignored()
        {
            var core = NewCore();
            var ctx = new FakeSkillContext();
            core.Reset(new[] { speedSkill, slamSkill, blinkSkill }, ctx, initialLevel: 99);
            int usedCount = 0;
            EventBus<SkillUsedEvent>.Subscribe(_ => usedCount++);

            core.HandleUseRequested(-1, ctx);
            core.HandleUseRequested(99, ctx);

            Assert.AreEqual(0, usedCount);
        }

        [Test]
        public void Reset_clears_previously_unlocked_state()
        {
            var core = NewCore();
            var ctx = new FakeSkillContext();
            core.Reset(new[] { speedSkill, slamSkill, blinkSkill }, ctx, initialLevel: 99);
            Assert.IsTrue(core.Runtimes[2].Unlocked);

            core.Reset(new[] { speedSkill, slamSkill, blinkSkill }, ctx, initialLevel: 1);
            Assert.IsFalse(core.Runtimes[2].Unlocked);
        }

        [Test]
        public void Null_skill_in_array_is_skipped()
        {
            var core = NewCore();
            var ctx = new FakeSkillContext();
            core.Reset(new SkillData[] { null, speedSkill, null }, ctx, initialLevel: 1);

            Assert.AreEqual(1, core.Runtimes.Count);
            Assert.AreSame(speedSkill, core.Runtimes[0].Data);
        }
    }
}
