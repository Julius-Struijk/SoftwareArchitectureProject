using System.Reflection;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using CMGTSA.Core;
using CMGTSA.Player;
using CMGTSA.UI;

namespace CMGTSA.Tests
{
    public class HUDLevelTextPresenterTests
    {
        private GameObject host;
        private TextMeshProUGUI label;
        private HUDLevelTextPresenter presenter;

        [SetUp]
        public void SetUp()
        {
            EventBus<PlayerLeveledUpEvent>.Clear();

            host = new GameObject("level-text-host");
            host.SetActive(false);

            label = host.AddComponent<TextMeshProUGUI>();
            presenter = host.AddComponent<HUDLevelTextPresenter>();
            SetPrivateField(presenter, "label", label);

            host.SetActive(true);  // OnEnable → Subscribe
        }

        [TearDown]
        public void TearDown()
        {
            if (host != null) Object.DestroyImmediate(host);
            EventBus<PlayerLeveledUpEvent>.Clear();
        }

        [Test]
        public void OnLeveledUp_writesLevelLabel()
        {
            EventBus<PlayerLeveledUpEvent>.Publish(new PlayerLeveledUpEvent(
                newLevel: 3, xpForNextLevel: 15));

            Assert.AreEqual("Lv. 3", label.text);
        }

        [Test]
        public void OnLeveledUp_followsLatestLevel()
        {
            EventBus<PlayerLeveledUpEvent>.Publish(new PlayerLeveledUpEvent(2, 10));
            EventBus<PlayerLeveledUpEvent>.Publish(new PlayerLeveledUpEvent(4, 20));

            Assert.AreEqual("Lv. 4", label.text);
        }

        [Test]
        public void OnLeveledUp_withNullLabel_doesNotThrow()
        {
            SetPrivateField(presenter, "label", null);

            Assert.DoesNotThrow(() =>
                EventBus<PlayerLeveledUpEvent>.Publish(new PlayerLeveledUpEvent(2, 10)));
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
