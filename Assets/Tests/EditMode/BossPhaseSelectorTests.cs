using NUnit.Framework;
using UnityEngine;
using CMGTSA.Boss;

namespace CMGTSA.Tests.EditMode
{
    public class BossPhaseSelectorTests
    {
        private BossPhase MakePhase(float threshold)
        {
            var p = ScriptableObject.CreateInstance<BossPhase>();
            p.hpThresholdEnter = threshold;
            p.castIntervalSeconds = 1f;
            p.patterns = new SOBossAttackPattern[0];
            return p;
        }

        private BossPhase[] phases;

        [SetUp]
        public void SetUp()
        {
            phases = new[] { MakePhase(1.0f), MakePhase(0.66f), MakePhase(0.33f) };
        }

        [TearDown]
        public void TearDown()
        {
            for (int i = 0; i < phases.Length; i++)
            {
                Object.DestroyImmediate(phases[i]);
            }
        }

        [Test]
        public void SelectPhase_FullHP_ReturnsFirstPhase()
        {
            var sel = new BossPhaseSelector(phases);
            Assert.AreEqual(0, sel.SelectPhase(1f));
        }

        [Test]
        public void SelectPhase_AtSecondThreshold_ReturnsSecondPhase()
        {
            var sel = new BossPhaseSelector(phases);
            Assert.AreEqual(1, sel.SelectPhase(0.66f));
        }

        [Test]
        public void SelectPhase_BetweenSecondAndThird_StaysOnSecond()
        {
            var sel = new BossPhaseSelector(phases);
            Assert.AreEqual(1, sel.SelectPhase(0.5f));
        }

        [Test]
        public void SelectPhase_AtThirdThreshold_ReturnsThirdPhase()
        {
            var sel = new BossPhaseSelector(phases);
            Assert.AreEqual(2, sel.SelectPhase(0.33f));
        }

        [Test]
        public void SelectPhase_BelowAllThresholds_StaysOnLast()
        {
            var sel = new BossPhaseSelector(phases);
            Assert.AreEqual(2, sel.SelectPhase(0f));
        }

        [Test]
        public void HasChangedSince_DetectsTransition()
        {
            var sel = new BossPhaseSelector(phases);
            Assert.IsFalse(sel.HasChangedSince(0, 1f));
            Assert.IsTrue(sel.HasChangedSince(0, 0.66f));
            Assert.IsTrue(sel.HasChangedSince(1, 0.33f));
            Assert.IsFalse(sel.HasChangedSince(2, 0.1f));
        }

        [Test]
        public void Constructor_NullArray_ProducesSingleZeroPhase()
        {
            var sel = new BossPhaseSelector(null);
            Assert.AreEqual(0, sel.SelectPhase(1f));
            Assert.AreEqual(0, sel.SelectPhase(0f));
        }
    }
}
