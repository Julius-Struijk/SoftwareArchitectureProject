using UnityEngine;

namespace CMGTSA.Skills
{
    [CreateAssetMenu(fileName = "BlinkEffect", menuName = "Scriptable Objects/Skills/BlinkEffect")]
    public class BlinkEffect : SOSkillEffect
    {
        [Tooltip("World-units travelled per blink along PlayerFacing.")]
        [Min(0.1f)] public float distance = 4f;

        public override bool IsPassive => false;

        public override void Activate(ISkillContext ctx)
        {
            if (ctx == null) return;
            Vector2 facing = ctx.PlayerFacing.sqrMagnitude > 0.0001f
                ? ctx.PlayerFacing.normalized
                : Vector2.right;
            Vector3 destination = ctx.PlayerPosition + (Vector3)(facing * distance);
            ctx.TeleportPlayer(destination);
        }
    }
}
