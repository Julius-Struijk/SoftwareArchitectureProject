using UnityEngine;
using CMGTSA.Battle;

namespace CMGTSA.Skills
{
    [CreateAssetMenu(fileName = "AOESlamEffect", menuName = "Scriptable Objects/Skills/AOESlamEffect")]
    public class AOESlamEffect : SOSkillEffect
    {
        [Tooltip("Overlap radius in world units.")]
        [Min(0.1f)] public float radius = 2.5f;

        [Tooltip("DamageData applied to every enemy in the radius.")]
        public DamageData damage = new DamageData { damage = 3 };

        public override bool IsPassive => false;

        public override void Activate(ISkillContext ctx)
        {
            if (ctx == null || damage == null) return;
            ctx.ApplyAreaDamage(ctx.PlayerPosition, radius, damage);
        }
    }
}
