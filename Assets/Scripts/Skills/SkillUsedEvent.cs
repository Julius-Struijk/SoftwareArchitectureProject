using CMGTSA.Core;

namespace CMGTSA.Skills
{
    public readonly struct SkillUsedEvent : IGameEvent
    {
        public readonly SkillData Skill;
        public readonly int SlotIndex;
        public readonly float CooldownDuration;

        public SkillUsedEvent(SkillData skill, int slotIndex, float cooldownDuration)
        {
            Skill = skill;
            SlotIndex = slotIndex;
            CooldownDuration = cooldownDuration;
        }
    }
}
