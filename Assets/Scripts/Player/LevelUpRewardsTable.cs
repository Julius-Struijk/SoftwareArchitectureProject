using System;
using UnityEngine;

namespace CMGTSA.Player
{
    /// <summary>
    /// Designer-tunable per-level rewards applied by <see cref="PlayerController"/>
    /// when it observes a <see cref="PlayerLeveledUpEvent"/>. Linear-scan lookup
    /// keeps the asset readable; <see cref="fallback"/> covers any level past the
    /// authored array (e.g. level 6+).
    /// </summary>
    [CreateAssetMenu(fileName = "LevelUpRewardsTable",
                     menuName = "CMGTSA/Player/LevelUpRewardsTable")]
    public class LevelUpRewardsTable : ScriptableObject
    {
        [Serializable]
        public struct LevelReward
        {
            [Tooltip("The level just reached (e.g. 2 means the reward applied when crossing 1->2).")]
            public int level;

            [Tooltip("How much MaxHP increases. CurrentHP is not auto-raised; pair with healToFull for full restore.")]
            public int hpDelta;

            [Tooltip("How much CurrentAttackDamage increases.")]
            public int damageDelta;

            [Tooltip("If true, the player heals to full MaxHP after the deltas are applied.")]
            public bool healToFull;
        }

        [Tooltip("Per-level entries. Linear scan; first matching level wins.")]
        public LevelReward[] rewards;

        [Tooltip("Used when the level just reached is not present in 'rewards' (e.g. level 6+).")]
        public LevelReward fallback;

        /// <summary>
        /// Returns the reward for the given level, or <see cref="fallback"/> if
        /// the level is not in <see cref="rewards"/>. Caller is responsible for
        /// guarding against the level-1 republish (see <see cref="PlayerController"/>).
        /// </summary>
        public LevelReward Get(int level)
        {
            if (rewards != null)
            {
                for (int i = 0; i < rewards.Length; i++)
                {
                    if (rewards[i].level == level) return rewards[i];
                }
            }
            return fallback;
        }
    }
}
