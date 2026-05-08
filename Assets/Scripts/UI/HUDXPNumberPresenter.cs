using UnityEngine;
using TMPro;
using CMGTSA.Core;
using CMGTSA.Player;

namespace CMGTSA.UI
{
    /// <summary>
    /// MVP presenter: subscribes to <see cref="PlayerXPGainedEvent"/> and writes
    /// "{xp} / {next}" into a <see cref="TextMeshProUGUI"/>. Pairs with
    /// <see cref="HUDXPBarPresenter"/> on the same canvas — two views, two
    /// presenters, one shared model, mirroring the HP-bar / HP-number layout.
    /// </summary>
    public class HUDXPNumberPresenter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI label;

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
            if (label == null) return;
            label.text = $"{evt.TotalXP} / {evt.XPForNextLevel}";
        }
    }
}
