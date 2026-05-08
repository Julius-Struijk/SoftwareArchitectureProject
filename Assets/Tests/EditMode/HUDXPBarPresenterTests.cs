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
    /// Verifies the XP bar presenter listens to the post-settlement
    /// PlayerXPGainedEvent and writes Image.fillAmount = xp / xpForNextLevel.
    /// Uses an active GameObject so AddComponent triggers OnEnable
    /// (which is where the presenter subscribes to the bus).
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

            // Create inactive so AddComponent defers Awake/OnEnable until after
            // all fields are set — Unity's EditMode test runner does not call
            // OnEnable during AddComponent on an already-active GameObject.
            host = new GameObject("xp-bar-host");
            host.SetActive(false);

            image = host.AddComponent<Image>();
            image.type = Image.Type.Filled;
            image.fillMethod = Image.FillMethod.Horizontal;
            image.fillAmount = 1f;

            presenter = host.AddComponent<HUDXPBarPresenter>();
            SetPrivateField(presenter, "fillImage", image);

            host.SetActive(true);  // Awake → fillImage guard; OnEnable → Subscribe
        }

        [TearDown]
        public void TearDown()
        {
            if (host != null) Object.DestroyImmediate(host);
            EventBus<PlayerXPGainedEvent>.Clear();
        }

        [Test]
        public void OnXPGained_setsFillAmountToRatio()
        {
            EventBus<PlayerXPGainedEvent>.Publish(new PlayerXPGainedEvent(
                totalXP: 3, gained: 1, xpForNextLevel: 5));

            Assert.AreEqual(0.6f, image.fillAmount, 0.0001f);
        }

        [Test]
        public void OnXPGained_clampsFillAmountToOne_whenXPMeetsThreshold()
        {
            EventBus<PlayerXPGainedEvent>.Publish(new PlayerXPGainedEvent(
                totalXP: 5, gained: 5, xpForNextLevel: 5));

            Assert.AreEqual(1f, image.fillAmount, 0.0001f);
        }

        [Test]
        public void OnXPGained_withZeroDenominator_setsFillAmountToZero()
        {
            EventBus<PlayerXPGainedEvent>.Publish(new PlayerXPGainedEvent(
                totalXP: 3, gained: 0, xpForNextLevel: 0));

            Assert.AreEqual(0f, image.fillAmount, 0.0001f);
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
    }
}
