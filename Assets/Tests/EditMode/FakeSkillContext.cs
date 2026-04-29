using System.Collections.Generic;
using UnityEngine;
using CMGTSA.Battle;
using CMGTSA.Skills;

namespace CMGTSA.Tests
{
    public class FakeSkillContext : ISkillContext
    {
        public Vector3 PlayerPosition { get; set; } = Vector3.zero;
        public Vector2 PlayerFacing { get; set; } = Vector2.right;

        public readonly List<float> SpeedMultiplierCalls = new List<float>();

        public struct AreaDamageCall { public Vector3 origin; public float radius; public DamageData damage; }
        public readonly List<AreaDamageCall> AreaDamageCalls = new List<AreaDamageCall>();

        public readonly List<Vector3> TeleportCalls = new List<Vector3>();

        public void ApplySpeedMultiplier(float multiplier)
        {
            SpeedMultiplierCalls.Add(multiplier);
        }

        public void ApplyAreaDamage(Vector3 origin, float radius, DamageData damage)
        {
            AreaDamageCalls.Add(new AreaDamageCall { origin = origin, radius = radius, damage = damage });
        }

        public void TeleportPlayer(Vector3 destination)
        {
            TeleportCalls.Add(destination);
        }
    }
}
