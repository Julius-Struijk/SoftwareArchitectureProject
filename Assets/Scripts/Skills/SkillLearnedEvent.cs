using CMGTSA.Core;

namespace CMGTSA.Skills
{
    public readonly struct SkillLearnedEvent : IGameEvent
    {
        public readonly SkillData Skill;
        public readonly int SlotIndex;

        public SkillLearnedEvent(SkillData skill, int slotIndex)
        {
            Skill = skill;
            SlotIndex = slotIndex;
        }
    }
}
