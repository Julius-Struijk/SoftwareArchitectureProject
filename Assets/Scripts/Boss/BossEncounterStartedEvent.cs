using CMGTSA.Core;

namespace CMGTSA.Boss
{
    public readonly struct BossEncounterStartedEvent : IGameEvent
    {
        public readonly BossData Data;

        public BossEncounterStartedEvent(BossData data)
        {
            Data = data;
        }
    }
}
