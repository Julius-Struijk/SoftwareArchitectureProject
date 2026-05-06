using CMGTSA.Core;

namespace CMGTSA.Boss
{
    public readonly struct BossEncounterEndedEvent : IGameEvent
    {
        public readonly BossData Data;
        public readonly bool Victory;

        public BossEncounterEndedEvent(BossData data, bool victory)
        {
            Data = data;
            Victory = victory;
        }
    }
}
