using UnityEngine;
using UnityEngine.InputSystem;
using CMGTSA.Core;

namespace CMGTSA.Skills
{
    public class SkillUseInput : MonoBehaviour
    {
        private void Update()
        {
            if (Keyboard.current == null) return;
            if (Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                EventBus<SkillUseRequestedEvent>.Publish(new SkillUseRequestedEvent(0));
            }
            if (Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                EventBus<SkillUseRequestedEvent>.Publish(new SkillUseRequestedEvent(1));
            }
            if (Keyboard.current.digit3Key.wasPressedThisFrame)
            {
                EventBus<SkillUseRequestedEvent>.Publish(new SkillUseRequestedEvent(2));
            }
        }
    }
}
