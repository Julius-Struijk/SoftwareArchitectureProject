using UnityEngine;
using CMGTSA.Battle;

namespace CMGTSA.Skills
{
    public interface ISkillContext
    {
        Vector3 PlayerPosition { get; }
        Vector2 PlayerFacing { get; }

        void ApplySpeedMultiplier(float multiplier);
        void ApplyAreaDamage(Vector3 origin, float radius, DamageData damage);
        void TeleportPlayer(Vector3 destination);
    }
}
