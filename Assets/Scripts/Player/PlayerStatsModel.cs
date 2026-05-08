using UnityEngine;
using CMGTSA.Core;

namespace CMGTSA.Player
{
    /// <summary>
    /// Plain-C# stats model. The "M" in five MVP relationships (HP bar, HP number,
    /// XP bar, XP number, level text — all driven from this single model via the
    /// event bus). Publishes <see cref="PlayerHPChangedEvent"/>,
    /// <see cref="PlayerXPGainedEvent"/>, <see cref="PlayerLeveledUpEvent"/>, and
    /// <see cref="PlayerDiedEvent"/> on the bus when its state changes.
    ///
    /// Level-up formula: the threshold to reach the next level is <c>5 * currentLevel</c>
    /// total XP. Picked deliberately small so first-minute level-up (rubric wow factor)
    /// happens after roughly two enemy kills.
    ///
    /// CurrentAttackDamage is the runtime damage value that <see cref="PlayerController"/>
    /// composes into each attack's <see cref="CMGTSA.Battle.DamageData"/>. Level-ups
    /// raise it via <see cref="IncreaseAttackDamage"/>.
    /// </summary>
    public class PlayerStatsModel
    {
        public int MaxHP { get; private set; }
        public int CurrentHP { get; private set; }
        public int XP { get; private set; }
        public int Level { get; private set; }
        public int Money { get; private set; }
        public int CurrentAttackDamage { get; private set; }

        public PlayerStatsModel(int maxHP, int startingMoney = 0, int baseDamage = 0)
        {
            MaxHP = maxHP;
            CurrentHP = maxHP;
            Level = 1;
            XP = 0;
            Money = startingMoney;
            CurrentAttackDamage = baseDamage;
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

        /// <summary>
        /// Raises the HP cap by <paramref name="delta"/>. CurrentHP is intentionally
        /// not auto-raised — keeping CurrentHP fixed on a MaxHP bump matches standard
        /// RPG convention; the heal step happens via <see cref="Heal"/> when the
        /// reward table specifies <c>healToFull</c>. Publishes a zero-delta
        /// <see cref="PlayerHPChangedEvent"/> so the HP-bar denominator updates.
        /// </summary>
        public void IncreaseMaxHP(int delta)
        {
            if (delta <= 0) return;
            MaxHP += delta;
            EventBus<PlayerHPChangedEvent>.Publish(
                new PlayerHPChangedEvent(CurrentHP, MaxHP, 0));
        }

        /// <summary>
        /// Raises the runtime attack damage by <paramref name="delta"/>. No event
        /// is published — there is no UI consumer today. (A future stats panel
        /// could subscribe to a new event if desired.)
        /// </summary>
        public void IncreaseAttackDamage(int delta)
        {
            if (delta <= 0) return;
            CurrentAttackDamage += delta;
        }

        public void GainXP(int amount)
        {
            if (amount <= 0) return;
            XP += amount;

            while (XP >= XPForNextLevel())
            {
                XP -= XPForNextLevel();
                Level++;
                EventBus<PlayerLeveledUpEvent>.Publish(
                    new PlayerLeveledUpEvent(Level, XPForNextLevel()));
            }

            EventBus<PlayerXPGainedEvent>.Publish(
                new PlayerXPGainedEvent(XP, amount, XPForNextLevel()));
        }

        public void GainMoney(int amount)
        {
            if (amount <= 0) return;
            Money += amount;
        }

        public int XPForNextLevel() => 5 * Level;
    }
}
