using UnityEngine;
using CMGTSA.Core;
using CMGTSA.Skills;

namespace CMGTSA.Feel
{
    /// <summary>
    /// Slice-7 polish (Tier 2): on <see cref="SkillUsedEvent"/>, spawns the SO-configured
    /// VFX prefab at the player's position. Lives on the player prefab so we don't need
    /// a player look-up; the controller's transform is the spawn origin.
    /// </summary>
    public class SkillCastVFXController : MonoBehaviour
    {
        private void OnEnable()
        {
            EventBus<SkillUsedEvent>.Subscribe(OnSkillUsed);
        }

        private void OnDisable()
        {
            EventBus<SkillUsedEvent>.Unsubscribe(OnSkillUsed);
        }

        private void OnSkillUsed(SkillUsedEvent evt)
        {
            if (evt.Skill == null || evt.Skill.OnUsedVFXPrefab == null) return;
            Instantiate(evt.Skill.OnUsedVFXPrefab, transform.position, Quaternion.identity);
        }
    }
}
