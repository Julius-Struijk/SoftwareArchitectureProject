using UnityEngine;

namespace CMGTSA.Boss
{
    [CreateAssetMenu(fileName = "BossData", menuName = "CMGTSA/Boss/Boss Data")]
    public class BossData : ScriptableObject
    {
        public string displayName = "Boss";
        [Min(1)] public int maxHP = 100;
        [Min(0.1f)] public float moveSpeed = 2f;
        [Min(0.5f)] public float engagementRange = 4f;

        [Tooltip("Phases in order, highest hpThresholdEnter first.")]
        public BossPhase[] phases;

        [Min(0)] public int xpReward = 100;
        [Min(0)] public int moneyReward = 50;
    }
}
