using UnityEngine;
#if UNITY_EDITOR
using UnityEngine.InputSystem;
using CMGTSA.Core;
#endif

namespace CMGTSA.Game
{
    public class DebugInputController : MonoBehaviour
    {
#if UNITY_EDITOR
        private void Update()
        {
            var kb = Keyboard.current;
            if (kb == null) return;

            if (kb.f1Key.wasPressedThisFrame)
            {
                DebugOptions.GodMode = !DebugOptions.GodMode;
                EventBus<DebugCheatToggledEvent>.Publish(
                    new DebugCheatToggledEvent(DebugCheat.GodModeToggle));
                Debug.Log($"[DebugOptions] GodMode = {DebugOptions.GodMode}");
            }
            if (kb.f2Key.wasPressedThisFrame)
            {
                EventBus<DebugCheatToggledEvent>.Publish(
                    new DebugCheatToggledEvent(DebugCheat.InstantKill));
                Debug.Log("[DebugOptions] InstantKill broadcast");
            }
            if (kb.f3Key.wasPressedThisFrame)
            {
                EventBus<DebugCheatToggledEvent>.Publish(
                    new DebugCheatToggledEvent(DebugCheat.TriggerGameOver));
                Debug.Log("[DebugOptions] TriggerGameOver");
            }
            if (kb.f4Key.wasPressedThisFrame)
            {
                EventBus<DebugCheatToggledEvent>.Publish(
                    new DebugCheatToggledEvent(DebugCheat.GiveMoney, DebugOptions.GiveMoneyAmount));
                Debug.Log($"[DebugOptions] GiveMoney +{DebugOptions.GiveMoneyAmount}");
            }
            if (kb.f5Key.wasPressedThisFrame)
            {
                EventBus<DebugCheatToggledEvent>.Publish(
                    new DebugCheatToggledEvent(DebugCheat.GiveXP, DebugOptions.GiveXPAmount));
                Debug.Log($"[DebugOptions] GiveXP +{DebugOptions.GiveXPAmount}");
            }
        }
#endif
    }
}
