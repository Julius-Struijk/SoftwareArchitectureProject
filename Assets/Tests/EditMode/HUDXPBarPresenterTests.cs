using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using CMGTSA.Core;
using CMGTSA.Player;
using CMGTSA.UI;

namespace CMGTSA.Tests
{
    /// <summary>
    /// Verifies the XP bar presenter listens to PlayerXPGainedEvent and drives
    /// the fill by setting RectTransform.anchorMax.x = xp / xpForNextLevel
    /// (same approach as the HP bar presenter).
    /// </summary>
    public class HUDXPBarPresenterTests
    {
        private GameObject host;
        private Image image;
        private HUDXPBarPresenter presenter;

        [SetUp]
        public void SetUp()
        {
            EventBus<PlayerXPGainedEvent>.Clear();

            host = new GameObject("xp-bar-host");
            image = host.AddComponent<Image>();

            presenter = host.AddComponent<HUDXPBarPresenter>();
            SetPrivateField(presenter, "fillImage", image);
            // Unity's EditMode NUnit runner does not call OnEnable during
            // AddComponent or SetActive. Invoke it directly so the presenter
            // subscribes to the bus before the test publishes events.
            InvokePrivate(presenter, "OnEnable");
        }

        [TearDown]
        public void TearDown()
        {
            if (host != null) Object.DestroyImmediate(host);
            EventBus<PlayerXPGainedEvent>.Clear();
        }

        [Test]
        public void OnXPGained_setsAnchorMaxXToRatio()
        {
            EventBus<PlayerXPGainedEvent>.Publish(new PlayerXPGainedEvent(
                totalXP: 3, gained: 1, xpForNextLevel: 5));

            Assert.AreEqual(0.6f, image.rectTransform.anchorMax.x, 0.0001f);
        }

        [Test]
        public void OnXPGained_clampsAnchorMaxToOne_whenXPMeetsThreshold()
        {
            EventBus<PlayerXPGainedEvent>.Publish(new PlayerXPGainedEvent(
                totalXP: 5, gained: 5, xpForNextLevel: 5));

            Assert.AreEqual(1f, image.rectTransform.anchorMax.x, 0.0001f);
        }

        [Test]
        public void OnXPGained_withZeroDenominator_setsAnchorMaxToZero()
        {
            EventBus<PlayerXPGainedEvent>.Publish(new PlayerXPGainedEvent(
                totalXP: 3, gained: 0, xpForNextLevel: 0));

            Assert.AreEqual(0f, image.rectTransform.anchorMax.x, 0.0001f);
        }

        [Test]
        public void OnXPGained_withNullFillImage_doesNotThrow()
        {
            SetPrivateField(presenter, "fillImage", null);

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

        private static void InvokePrivate(object target, string name)
        {
            target.GetType()
                .GetMethod(name, BindingFlags.NonPublic | BindingFlags.Instance)
                ?.Invoke(target, null);
        }
    }
}
