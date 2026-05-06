using UnityEngine;
using CMGTSA.Battle;
using CMGTSA.Boss;
using CMGTSA.Core;
using CMGTSA.Enemies;
using CMGTSA.Inventory;
using CMGTSA.Player;
using CMGTSA.Skills;

namespace CMGTSA.Audio
{
    /// <summary>
    /// Slice-7 polish: routes every gameplay event that should make a sound through a
    /// single AudioSource. Source clips are looked up in <see cref="AudioCatalog"/> by
    /// <see cref="AudioSlot"/>, so retiming a sound or swapping a clip is a designer-only
    /// edit on the catalog asset — no code change.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class SFXController : MonoBehaviour
    {
        [SerializeField] private AudioCatalog catalog;
        private AudioSource source;

        private void Awake()
        {
            source = GetComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = false;
        }

        private void OnEnable()
        {
            EventBus<PlayerAttackRequestedEvent>.Subscribe(OnPlayerAttack);
            EventBus<DamageDealtEvent>.Subscribe(OnDamageDealt);
            EventBus<EnemyDiedEvent>.Subscribe(OnEnemyDied);
            EventBus<PlayerDiedEvent>.Subscribe(OnPlayerDied);
            EventBus<PlayerLeveledUpEvent>.Subscribe(OnLeveledUp);
            EventBus<SkillUsedEvent>.Subscribe(OnSkillUsed);
            EventBus<BossEncounterStartedEvent>.Subscribe(OnBossStart);
            EventBus<ItemUsedEvent>.Subscribe(OnItemUsed);
        }

        private void OnDisable()
        {
            EventBus<PlayerAttackRequestedEvent>.Unsubscribe(OnPlayerAttack);
            EventBus<DamageDealtEvent>.Unsubscribe(OnDamageDealt);
            EventBus<EnemyDiedEvent>.Unsubscribe(OnEnemyDied);
            EventBus<PlayerDiedEvent>.Unsubscribe(OnPlayerDied);
            EventBus<PlayerLeveledUpEvent>.Unsubscribe(OnLeveledUp);
            EventBus<SkillUsedEvent>.Unsubscribe(OnSkillUsed);
            EventBus<BossEncounterStartedEvent>.Unsubscribe(OnBossStart);
            EventBus<ItemUsedEvent>.Unsubscribe(OnItemUsed);
        }

        private void Play(AudioSlot slot)
        {
            if (catalog == null) return;
            if (catalog.TryGet(slot, out var clip, out var volume))
            {
                source.PlayOneShot(clip, volume);
            }
        }

        private void OnPlayerAttack(PlayerAttackRequestedEvent _) => Play(AudioSlot.AttackWhoosh);

        private void OnDamageDealt(DamageDealtEvent evt)
        {
            bool playerWasHit = evt.Target != null && evt.Target.CompareTag("Player");
            Play(playerWasHit ? AudioSlot.PlayerHit : AudioSlot.EnemyHit);
        }

        private void OnEnemyDied(EnemyDiedEvent _)            => Play(AudioSlot.EnemyDeath);
        private void OnPlayerDied(PlayerDiedEvent _)          => Play(AudioSlot.PlayerDeath);
        private void OnLeveledUp(PlayerLeveledUpEvent _)      => Play(AudioSlot.LevelUp);
        private void OnSkillUsed(SkillUsedEvent _)            => Play(AudioSlot.SkillCast);
        private void OnBossStart(BossEncounterStartedEvent _) => Play(AudioSlot.BossStinger);
        private void OnItemUsed(ItemUsedEvent _)              => Play(AudioSlot.UIClick);
    }
}
