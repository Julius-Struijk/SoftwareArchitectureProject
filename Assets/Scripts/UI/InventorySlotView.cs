using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CMGTSA.Inventory;

namespace CMGTSA.UI
{
    /// <summary>
    /// One inventory slot cell. Pure View — wired by <see cref="InventoryPresenter"/> at
    /// runtime, never reads model state itself. Click invokes the supplied callback so the
    /// presenter can route to <c>InventoryModel.UseSlot(int)</c>.
    /// </summary>
    public class InventorySlotView : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI countLabel;
        [SerializeField] private Image equippedBorder;
        [SerializeField] private Button button;

        private System.Action onClick;

        private void Awake()
        {
            if (button != null) button.onClick.AddListener(HandleClick);
        }

        private void OnDestroy()
        {
            if (button != null) button.onClick.RemoveListener(HandleClick);
        }

        private void HandleClick() => onClick?.Invoke();

        public void Bind(InventorySlot slot, System.Action onClickCallback)
        {
            onClick = onClickCallback;

            if (iconImage != null)
            {
                iconImage.sprite = slot.Item.icon;
                iconImage.enabled = slot.Item.icon != null;
            }
            if (countLabel != null)
            {
                countLabel.text = slot.Count > 1 ? slot.Count.ToString() : string.Empty;
            }
            if (equippedBorder != null)
            {
                equippedBorder.enabled = slot.Equipped;
            }
        }
    }
}
