using UnityEngine;

namespace CMGTSA.Skills
{
    [CreateAssetMenu(fileName = "SkillData", menuName = "Scriptable Objects/Skills/SkillData")]
    public class SkillData : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Player-facing name shown in the skill panel tooltip.")]
        public string displayName;

        [Tooltip("Sprite displayed in the slot icon.")]
        public Sprite icon;

        [TextArea]
        [Tooltip("Optional flavour text for the skill panel.")]
        public string description;

        [Header("Unlock + activation")]
        [Tooltip("Player level required to unlock this skill.")]
        [Min(1)] public int requiredLevel = 1;

        [Tooltip("Seconds between activations. Ignored for passives.")]
        [Min(0f)] public float cooldownSeconds = 4f;

        [Header("Behaviour")]
        [Tooltip("Polymorphic SO Strategy.")]
        public SOSkillEffect effect;

        public bool IsPassive => effect != null && effect.IsPassive;

        public SkillRuntime CreateRuntime() => new SkillRuntime(this);
    }
}
