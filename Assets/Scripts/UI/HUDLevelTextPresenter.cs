using UnityEngine;
using TMPro;
using CMGTSA.Core;
using CMGTSA.Player;

namespace CMGTSA.UI
{
    /// <summary>
    /// MVP presenter: subscribes to <see cref="PlayerLeveledUpEvent"/> and writes
    /// "Lv. {n}" into a <see cref="TextMeshProUGUI"/>. PlayerController re-publishes
    /// PlayerLeveledUpEvent in Start so the label shows "Lv. 1" at game-open
    /// before any real level-up.
    /// </summary>
    public class HUDLevelTextPresenter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI label;

        private void OnEnable()
        {
            EventBus<PlayerLeveledUpEvent>.Subscribe(OnLeveledUp);
        }

        private void OnDisable()
        {
            EventBus<PlayerLeveledUpEvent>.Unsubscribe(OnLeveledUp);
        }

        private void OnLeveledUp(PlayerLeveledUpEvent evt)
        {
            if (label == null) return;
            label.text = $"Lv. {evt.NewLevel}";
        }
    }
}
