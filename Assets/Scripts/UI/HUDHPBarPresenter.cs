using UnityEngine;
using UnityEngine.UI;
using CMGTSA.Core;
using CMGTSA.Player;

namespace CMGTSA.UI
{
    /// <summary>
    /// MVP presenter: subscribes to <see cref="PlayerHPChangedEvent"/> and drives the fill
    /// bar by scaling <see cref="fillImage"/>'s RectTransform via <c>anchorMax.x</c>.
    /// The fill Image must be anchored left-stretch inside the background rect (anchorMin.x=0,
    /// anchorMax.x=1 at full HP) so shrinking anchorMax.x clips from the right.
    /// </summary>
    public class HUDHPBarPresenter : MonoBehaviour
    {
        [SerializeField] private Image fillImage;

        private void Awake()
        {
            if (fillImage == null) fillImage = GetComponent<Image>();
        }

        private void OnEnable()
        {
            EventBus<PlayerHPChangedEvent>.Subscribe(OnHPChanged);
        }

        private void OnDisable()
        {
            EventBus<PlayerHPChangedEvent>.Unsubscribe(OnHPChanged);
        }

        private void OnHPChanged(PlayerHPChangedEvent evt)
        {
            if (fillImage == null) return;
            float fill = evt.MaxHP > 0 ? Mathf.Clamp01(evt.CurrentHP / (float)evt.MaxHP) : 0f;
            RectTransform rt = fillImage.rectTransform;
            rt.anchorMax = new Vector2(fill, rt.anchorMax.y);
        }
    }
}
