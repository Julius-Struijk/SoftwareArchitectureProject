using UnityEngine;
using CMGTSA.Core;

namespace CMGTSA.Player
{
    /// <summary>
    /// Plain-C# stats model. The "M" in three MVP relationships (HP bar, HP number,
    /// future inventory money/XP). Publishes <see cref="PlayerHPChangedEvent"/>,
    /// <see cref="PlayerXPGainedEvent"/>, <see cref="PlayerLeveledUpEvent"/>, and
    /// <see cref="PlayerDiedEvent"/> on the bus when its state changes.
    ///
    /// Level-up formula: the threshold to reach the next level is <c>5 * currentLevel</c>
    /// total XP. Picked deliberately small so first-minute level-up (rubric wow factor)
    /// happens after roughly two enemy kills.
    /// </summary>
    public class PlayerStatsModel
    {
        public int MaxHP { get; private set; }
        public int CurrentHP { get; private set; }
        public int XP { get; private set; }
        public int Level { get; private set; }
        public int Money { get; private set; }

        public PlayerStatsModel(int maxHP, int startingMoney = 0)
        {
            MaxHP = maxHP;
            CurrentHP = maxHP;
            Level = 1;
            XP = 0;
            Money = startingMoney;
        }

        public void Damage(int amount)
        {
            if (amount <= 0) return;
            if (CurrentHP <= 0) return;

            int prev = CurrentHP;
            CurrentHP = Mathf.Max(0, CurrentHP - amount);
            EventBus<PlayerHPChangedEvent>.Publish(
                new PlayerHPChangedEvent(CurrentHP, MaxHP, CurrentHP - prev));

            if (CurrentHP == 0)
            {
                EventBus<PlayerDiedEvent>.Publish(default);
            }
        }

        public void Heal(int amount)
        {
            if (amount <= 0) return;
            if (CurrentHP <= 0) return;

            int prev = CurrentHP;
            CurrentHP = Mathf.Min(MaxHP, CurrentHP + amount);
            if (CurrentHP == prev) return;
            EventBus<PlayerHPChangedEvent>.Publish(
                new PlayerHPChangedEvent(CurrentHP, MaxHP, CurrentHP - prev));
        }

        public void GainXP(int amount)
        {
            if (amount <= 0) return;
            XP += amount;
            EventBus<PlayerXPGainedEvent>.Publish(new PlayerXPGainedEvent(XP, amount));

            while (XP >= XPForNextLevel())
            {
                Level++;
                EventBus<PlayerLeveledUpEvent>.Publish(new PlayerLeveledUpEvent(Level));
            }
        }

        public void GainMoney(int amount)
        {
            if (amount <= 0) return;
            Money += amount;
        }

        public int XPForNextLevel() => 5 * Level;
    }
}
