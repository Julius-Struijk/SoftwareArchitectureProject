using System.Collections.Generic;
using UnityEngine;
using CMGTSA.Core;
using CMGTSA.Game;
using CMGTSA.Player;

namespace CMGTSA.Skills
{
    public class SkillManager : MonoBehaviour
    {
        [Header("Skills")]
        [Tooltip("All skills the player can learn this run. Order matters: first non-passive = key 1, second = key 2.")]
        [SerializeField] private SkillData[] availableSkills;

        [Header("Initial level")]
        [Min(1)] [SerializeField] private int initialLevel = 1;

        private PureCore core;
        private ISkillContext context;

        private void Awake()
        {
            core = new PureCore(() => Time.time);
        }

        private void OnEnable()
        {
            EventBus<GameRestartedEvent>.Subscribe(OnGameRestarted);
            EventBus<PlayerLeveledUpEvent>.Subscribe(OnPlayerLeveledUp);
            EventBus<SkillUseRequestedEvent>.Subscribe(OnUseRequested);
        }

        private void OnDisable()
        {
            EventBus<GameRestartedEvent>.Unsubscribe(OnGameRestarted);
            EventBus<PlayerLeveledUpEvent>.Unsubscribe(OnPlayerLeveledUp);
            EventBus<SkillUseRequestedEvent>.Unsubscribe(OnUseRequested);
        }

        private void ResolveContext()
        {
            if (context != null) return;
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go == null) return;
            var pc = go.GetComponent<PlayerController>();
            if (pc == null) return;
            context = pc.SkillContext;
        }

        private void OnGameRestarted(GameRestartedEvent _)
        {
            ResolveContext();
            core.Reset(availableSkills, context, initialLevel);
        }

        private void OnPlayerLeveledUp(PlayerLeveledUpEvent evt)
        {
            ResolveContext();
            core.HandleLevelUp(evt.NewLevel, context);
        }

        private void OnUseRequested(SkillUseRequestedEvent evt)
        {
            ResolveContext();
            core.HandleUseRequested(evt.SlotIndex, context);
        }

        public class PureCore
        {
            private readonly System.Func<float> nowProvider;
            private readonly List<SkillRuntime> runtimes = new List<SkillRuntime>();
            private readonly List<int> activeSlotToRuntimeIndex = new List<int>();

            public PureCore(System.Func<float> nowProvider)
            {
                this.nowProvider = nowProvider ?? (() => 0f);
            }

            public IReadOnlyList<SkillRuntime> Runtimes => runtimes;
            public int ActiveSlotCount => activeSlotToRuntimeIndex.Count;

            public void Reset(SkillData[] skills, ISkillContext ctx, int initialLevel)
            {
                runtimes.Clear();
                activeSlotToRuntimeIndex.Clear();
                if (skills == null) return;

                for (int i = 0; i < skills.Length; i++)
                {
                    if (skills[i] == null) continue;
                    var rt = skills[i].CreateRuntime();
                    int runtimeIndex = runtimes.Count;
                    runtimes.Add(rt);
                    if (!skills[i].IsPassive) activeSlotToRuntimeIndex.Add(runtimeIndex);
                }

                for (int i = 0; i < runtimes.Count; i++)
                {
                    var rt = runtimes[i];
                    if (rt.Data == null) continue;
                    if (rt.Data.requiredLevel <= initialLevel)
                    {
                        Unlock(rt, ctx);
                    }
                }
            }

            public void HandleLevelUp(int newLevel, ISkillContext ctx)
            {
                for (int i = 0; i < runtimes.Count; i++)
                {
                    var rt = runtimes[i];
                    if (rt.Data == null) continue;
                    if (rt.Unlocked) continue;
                    if (rt.Data.requiredLevel <= newLevel)
                    {
                        Unlock(rt, ctx);
                    }
                }
            }

            public void HandleUseRequested(int slotIndex, ISkillContext ctx)
            {
                if (slotIndex < 0 || slotIndex >= activeSlotToRuntimeIndex.Count) return;
                int runtimeIndex = activeSlotToRuntimeIndex[slotIndex];
                var rt = runtimes[runtimeIndex];
                if (rt.Data == null || rt.Data.effect == null) return;
                if (!rt.Unlocked) return;
                if (rt.IsOnCooldown(nowProvider())) return;

                rt.Data.effect.Activate(ctx);
                rt.CooldownEndTime = nowProvider() + rt.Data.cooldownSeconds;

                EventBus<SkillUsedEvent>.Publish(
                    new SkillUsedEvent(rt.Data, slotIndex, rt.Data.cooldownSeconds));
            }

            private void Unlock(SkillRuntime rt, ISkillContext ctx)
            {
                rt.Unlocked = true;
                if (rt.Data.effect != null) rt.Data.effect.OnLearned(ctx);
                int slotIndex = ResolveActiveSlotIndex(rt);
                EventBus<SkillLearnedEvent>.Publish(new SkillLearnedEvent(rt.Data, slotIndex));
            }

            private int ResolveActiveSlotIndex(SkillRuntime rt)
            {
                int runtimeIndex = runtimes.IndexOf(rt);
                for (int i = 0; i < activeSlotToRuntimeIndex.Count; i++)
                {
                    if (activeSlotToRuntimeIndex[i] == runtimeIndex) return i;
                }
                return -1;
            }
        }
    }
}
