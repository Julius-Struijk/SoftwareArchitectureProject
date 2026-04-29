using CMGTSA.Core;

namespace CMGTSA.Skills
{
    public readonly struct SkillUseRequestedEvent : IGameEvent
    {
        public readonly int SlotIndex;

        public SkillUseRequestedEvent(int slotIndex)
        {
            SlotIndex = slotIndex;
        }
    }
}
