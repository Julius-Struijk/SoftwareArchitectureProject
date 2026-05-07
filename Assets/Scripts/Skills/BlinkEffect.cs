using UnityEngine;

namespace CMGTSA.Skills
{
    [CreateAssetMenu(fileName = "BlinkEffect", menuName = "Scriptable Objects/Skills/BlinkEffect")]
    public class BlinkEffect : SOSkillEffect
    {
        [Tooltip("World-units travelled per blink along PlayerFacing.")]
        [Min(0.1f)] public float distance = 4f;

        [Tooltip("Layers the blink should not cross. Set to the Wall layer in the scene.")]
        public LayerMask wallMask;

        [Tooltip("World-units to subtract from the wall hit so the player lands just before the wall. Assumes walls are perpendicular to the blink direction (valid for axis-aligned tile dungeons).")]
        public float skin = 0.1f;

        public override bool IsPassive => false;

        public override void Activate(ISkillContext ctx)
        {
            if (ctx == null) return;

            Vector2 facing = ctx.PlayerFacing.sqrMagnitude > 0.0001f
                ? ctx.PlayerFacing.normalized
                : Vector2.right;

            Vector3 origin      = ctx.PlayerPosition;
            Vector3 destination = origin + (Vector3)(facing * distance);

            if (ctx.Physics != null && ctx.Physics.TryCast(origin, destination, wallMask, out Vector2 hitPoint))
            {
                Vector3 clamped = (Vector3)(hitPoint - facing * skin);
                clamped.z = origin.z;
                destination = clamped;
            }

            ctx.TeleportPlayer(destination);
        }
    }
}
