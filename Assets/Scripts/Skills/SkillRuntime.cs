namespace CMGTSA.Skills
{
    public class SkillRuntime
    {
        public SkillData Data { get; }
        public bool Unlocked;
        public float CooldownEndTime;

        public SkillRuntime(SkillData data)
        {
            Data = data;
            Unlocked = false;
            CooldownEndTime = 0f;
        }

        public bool IsOnCooldown(float now) => now < CooldownEndTime;
    }
}
