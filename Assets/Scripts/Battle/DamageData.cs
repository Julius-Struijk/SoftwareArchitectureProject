using UnityEngine;
using System;

namespace CMGTSA.Battle
{
    /// <summary>
    /// Damage data contains: damage, slowdown percentage, slowdown time(duration)
    /// and a damage type which is both a unique type ID and a VFX container(to be
    /// played on hit).
    /// </summary>
    [Serializable]
    public class DamageData
    {
        public int damage;
        public float slowDown;
        public float slowDownTime;
        public DamageType damageType;
    }
}
