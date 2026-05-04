using NUnit.Framework;
using CMGTSA.Boss;
using CMGTSA.Core;

namespace CMGTSA.Tests.EditMode
{
    public class BossHealthModelTests
    {
        private int eventCount;
        private BossHPChangedEvent lastEvent;

        [SetUp]
        public void SetUp()
        {
            eventCount = 0;
            EventBus<BossHPChangedEvent>.Subscribe(OnHPChanged);
        }

        [TearDown]
        public void TearDown()
        {
            EventBus<BossHPChangedEvent>.Unsubscribe(OnHPChanged);
        }

        private void OnHPChanged(BossHPChangedEvent evt)
        {
            eventCount++;
            lastEvent = evt;
        }

        [Test]
        public void Damage_ReducesCurrentAndPublishesEvent()
        {
            var model = new BossHealthModel(100, 0);
            model.Damage(20);

            Assert.AreEqual(80, model.Current);
            Assert.AreEqual(1, eventCount);
            Assert.AreEqual(80, lastEvent.Current);
            Assert.AreEqual(100, lastEvent.Max);
            Assert.AreEqual(0, lastEvent.PhaseIndex);
        }

        [Test]
        public void Damage_ClampsToZero()
        {
            var model = new BossHealthModel(10, 0);
            bool dead = model.Damage(50);

            Assert.AreEqual(0, model.Current);
            Assert.IsTrue(dead);
        }

        [Test]
        public void Damage_NonPositiveAmount_DoesNothing()
        {
            var model = new BossHealthModel(100, 0);
            model.Damage(0);
            model.Damage(-5);

            Assert.AreEqual(100, model.Current);
            Assert.AreEqual(0, eventCount);
        }

        [Test]
        public void SetPhaseIndex_NextDamagePublishesNewIndex()
        {
            var model = new BossHealthModel(100, 0);
            model.SetPhaseIndex(2);
            model.Damage(1);

            Assert.AreEqual(2, lastEvent.PhaseIndex);
        }
    }
}
