using System.Reflection;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using CMGTSA.Core;
using CMGTSA.Player;
using CMGTSA.UI;

namespace CMGTSA.Tests
{
    public class HUDXPNumberPresenterTests
    {
        private GameObject host;
        private TextMeshProUGUI label;
        private HUDXPNumberPresenter presenter;

        [SetUp]
        public void SetUp()
        {
            EventBus<PlayerXPGainedEvent>.Clear();

            host = new GameObject("xp-number-host");
            label = host.AddComponent<TextMeshProUGUI>();
            presenter = host.AddComponent<HUDXPNumberPresenter>();
            SetPrivateField(presenter, "label", label);
        }

        [TearDown]
        public void TearDown()
        {
            if (host != null) Object.DestroyImmediate(host);
            EventBus<PlayerXPGainedEvent>.Clear();
        }

        [Test]
        public void OnXPGained_writesCurrentSlashNext()
        {
            EventBus<PlayerXPGainedEvent>.Publish(new PlayerXPGainedEvent(
                totalXP: 3, gained: 1, xpForNextLevel: 5));

            Assert.AreEqual("3 / 5", label.text);
        }

        [Test]
        public void OnXPGained_afterLevelUp_reflectsNewDenominator()
        {
            EventBus<PlayerXPGainedEvent>.Publish(new PlayerXPGainedEvent(
                totalXP: 0, gained: 5, xpForNextLevel: 10));

            Assert.AreEqual("0 / 10", label.text);
        }

        [Test]
        public void OnXPGained_withNullLabel_doesNotThrow()
        {
            SetPrivateField(presenter, "label", null);

            Assert.DoesNotThrow(() =>
                EventBus<PlayerXPGainedEvent>.Publish(new PlayerXPGainedEvent(1, 1, 5)));
        }

        private static void SetPrivateField(object target, string name, object value)
        {
            FieldInfo f = target.GetType().GetField(name,
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(f, $"Field {name} not found on {target.GetType().Name}");
            f.SetValue(target, value);
        }
    }
}
