using UnityEngine;
using TMPro;
using CMGTSA.Core;
using CMGTSA.Player;

namespace CMGTSA.UI
{
    /// <summary>
    /// MVP presenter: subscribes to <see cref="PlayerHPChangedEvent"/> and writes the
    /// "current/max" string into a <see cref="TextMeshProUGUI"/>. Lives alongside the
    /// HP-bar presenter on the same HUD canvas — demonstrating two views, two presenters,
    /// one shared model.
    /// </summary>
    public class HUDHPNumberPresenter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI label;

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
            if (label == null) return;
            label.text = $"{evt.CurrentHP} / {evt.MaxHP}";
        }
    }
}
