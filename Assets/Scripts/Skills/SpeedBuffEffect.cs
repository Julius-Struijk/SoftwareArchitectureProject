using UnityEngine;

namespace CMGTSA.Skills
{
    [CreateAssetMenu(fileName = "SpeedBuffEffect", menuName = "Scriptable Objects/Skills/SpeedBuffEffect")]
    public class SpeedBuffEffect : SOSkillEffect
    {
        [Tooltip("Multiplier applied to PlayerController.MoveSpeed on learn. 1.25 = +25%.")]
        [Min(0.01f)] public float multiplier = 1.25f;

        public override bool IsPassive => true;

        public override void OnLearned(ISkillContext ctx)
        {
            if (ctx == null) return;
            ctx.ApplySpeedMultiplier(multiplier);
        }
    }
}
