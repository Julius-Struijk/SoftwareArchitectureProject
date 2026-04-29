using UnityEngine;

namespace CMGTSA.Skills
{
    public abstract class SOSkillEffect : ScriptableObject
    {
        public virtual bool IsPassive => false;
        public virtual void OnLearned(ISkillContext ctx) { }
        public virtual void Activate(ISkillContext ctx) { }
    }
}
