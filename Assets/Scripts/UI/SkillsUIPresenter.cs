using UnityEngine;
using CMGTSA.Core;
using CMGTSA.Game;
using CMGTSA.Skills;

namespace CMGTSA.UI
{
    public class SkillsUIPresenter : MonoBehaviour
    {
        [Tooltip("Slot views in active-slot order (slot 0 = key 1, slot 1 = key 2, slot 2 = key 3).")]
        [SerializeField] private SkillSlotView[] slots;

        private void OnEnable()
        {
            ResetSlots();
            EventBus<SkillLearnedEvent>.Subscribe(OnSkillLearned);
            EventBus<SkillUsedEvent>.Subscribe(OnSkillUsed);
            EventBus<GameRestartedEvent>.Subscribe(OnGameRestarted);
        }

        private void OnDisable()
        {
            EventBus<SkillLearnedEvent>.Unsubscribe(OnSkillLearned);
            EventBus<SkillUsedEvent>.Unsubscribe(OnSkillUsed);
            EventBus<GameRestartedEvent>.Unsubscribe(OnGameRestarted);
        }

        private void ResetSlots()
        {
            if (slots == null) return;
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] == null) continue;
                slots[i].Bind(null, i);
                slots[i].SetUnlocked(false);
            }
        }

        private void OnSkillLearned(SkillLearnedEvent evt)
        {
            if (evt.SlotIndex < 0) return;
            if (slots == null || evt.SlotIndex >= slots.Length) return;
            var slot = slots[evt.SlotIndex];
            if (slot == null) return;
            slot.Bind(evt.Skill, evt.SlotIndex);
            slot.SetUnlocked(true);
        }

        private void OnSkillUsed(SkillUsedEvent evt)
        {
            if (slots == null || evt.SlotIndex < 0 || evt.SlotIndex >= slots.Length) return;
            var slot = slots[evt.SlotIndex];
            if (slot == null) return;
            slot.StartCooldown(evt.CooldownDuration);
        }

        private void OnGameRestarted(GameRestartedEvent _)
        {
            ResetSlots();
        }
    }
}
