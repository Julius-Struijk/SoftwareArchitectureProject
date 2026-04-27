using UnityEngine;
using UnityEngine.UI;
using CMGTSA.Core;
using CMGTSA.Player;

namespace CMGTSA.UI
{
    /// <summary>
    /// MVP presenter: subscribes to <see cref="PlayerHPChangedEvent"/> and updates
    /// <see cref="fillImage"/>'s <c>fillAmount</c>. The Image's Image Type must be
    /// <c>Filled</c> with Horizontal fill method in the Inspector.
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
            fillImage.fillAmount = evt.MaxHP > 0
                ? Mathf.Clamp01(evt.CurrentHP / (float)evt.MaxHP)
                : 0f;
        }
    }
}
