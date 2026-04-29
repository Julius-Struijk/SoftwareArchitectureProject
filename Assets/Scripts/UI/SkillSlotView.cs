using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CMGTSA.Skills;

namespace CMGTSA.UI
{
    public class SkillSlotView : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private Image cooldownOverlay;
        [SerializeField] private TextMeshProUGUI keyLabel;
        [SerializeField] private Color lockedColor = new Color(0.3f, 0.3f, 0.3f, 0.6f);
        [SerializeField] private Color unlockedColor = Color.white;

        private float cooldownEndTime;
        private float cooldownDuration;
        private bool unlocked;

        public void Bind(SkillData skill, int slotIndex)
        {
            if (iconImage != null)
            {
                iconImage.sprite = skill != null ? skill.icon : null;
                iconImage.enabled = iconImage.sprite != null;
            }
            if (keyLabel != null) keyLabel.text = (slotIndex + 1).ToString();
            SetUnlocked(false);
        }

        public void SetUnlocked(bool isUnlocked)
        {
            unlocked = isUnlocked;
            if (iconImage != null) iconImage.color = unlocked ? unlockedColor : lockedColor;
            if (cooldownOverlay != null)
            {
                cooldownOverlay.fillAmount = unlocked ? 0f : 1f;
            }
        }

        public void StartCooldown(float duration)
        {
            cooldownDuration = Mathf.Max(0.0001f, duration);
            cooldownEndTime = Time.time + duration;
        }

        private void Update()
        {
            if (cooldownOverlay == null) return;
            if (!unlocked)
            {
                cooldownOverlay.fillAmount = 1f;
                return;
            }
            float remaining = cooldownEndTime - Time.time;
            if (remaining <= 0f)
            {
                cooldownOverlay.fillAmount = 0f;
                return;
            }
            cooldownOverlay.fillAmount = Mathf.Clamp01(remaining / cooldownDuration);
        }
    }
}
