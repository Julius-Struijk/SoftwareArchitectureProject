using UnityEngine;
using UnityEngine.UI;
using CMGTSA.Core;
using CMGTSA.Player;

namespace CMGTSA.UI
{
    /// <summary>
    /// MVP presenter: subscribes to <see cref="PlayerXPGainedEvent"/> and drives
    /// the XP fill bar by setting <see cref="Image.fillAmount"/>. The fill Image
    /// must be authored in the Inspector with <c>Image Type = Filled</c> and
    /// <c>Fill Method = Horizontal</c> so the value translates to a horizontal
    /// progress bar. The post-settlement <see cref="PlayerXPGainedEvent"/> always
    /// carries the correct denominator after a level-up, so this presenter only
    /// needs the one event subscription.
    /// </summary>
    public class HUDXPBarPresenter : MonoBehaviour
    {
        [SerializeField] private Image fillImage;

        private void Awake()
        {
            if (fillImage == null) fillImage = GetComponent<Image>();
        }

        private void OnEnable()
        {
            EventBus<PlayerXPGainedEvent>.Subscribe(OnXPGained);
        }

        private void OnDisable()
        {
            EventBus<PlayerXPGainedEvent>.Unsubscribe(OnXPGained);
        }

        private void OnXPGained(PlayerXPGainedEvent evt)
        {
            if (fillImage == null) return;
            float fill = evt.XPForNextLevel > 0
                ? Mathf.Clamp01(evt.TotalXP / (float)evt.XPForNextLevel)
                : 0f;
            fillImage.fillAmount = fill;
        }
    }
}
