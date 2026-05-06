using UnityEngine;
using CMGTSA.Battle;
using CMGTSA.Boss;
using CMGTSA.Core;
using CMGTSA.Player;

namespace CMGTSA.Feel
{
    /// <summary>
    /// Slice-7 polish: camera shake. Subscribes to multiple existing events and tunes the
    /// shake parameters per source. Applies an additive world-position offset in LateUpdate
    /// so it layers on top of CameraFollow without interfering with the camera's base position.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class ScreenShakeController : MonoBehaviour
    {
        [Header("Per-source magnitudes")]
        [SerializeField] private float hitMagnitude = 0.10f;
        [SerializeField] private float hitDuration  = 0.10f;
        [SerializeField] private float bossIntroMagnitude = 0.25f;
        [SerializeField] private float bossIntroDuration  = 0.40f;
        [SerializeField] private float deathMagnitude = 0.40f;
        [SerializeField] private float deathDuration  = 0.80f;

        [Tooltip("Higher = faster shake. 25 is a safe default for 2D pixel-art look.")]
        [SerializeField] private float frequency = 25f;

        private float magnitude;
        private float remaining;
        private float seed;

        private void Awake()
        {
            seed = Random.Range(0f, 1000f);
        }

        private void OnEnable()
        {
            EventBus<DamageDealtEvent>.Subscribe(OnDamageDealt);
            EventBus<BossEncounterStartedEvent>.Subscribe(OnBossStart);
            EventBus<PlayerDiedEvent>.Subscribe(OnPlayerDied);
        }

        private void OnDisable()
        {
            EventBus<DamageDealtEvent>.Unsubscribe(OnDamageDealt);
            EventBus<BossEncounterStartedEvent>.Unsubscribe(OnBossStart);
            EventBus<PlayerDiedEvent>.Unsubscribe(OnPlayerDied);
        }

        private void LateUpdate()
        {
            if (remaining <= 0f) return;

            remaining -= Time.unscaledDeltaTime;
            if (remaining <= 0f) return;

            float t = Time.unscaledTime * frequency;
            float x = (Mathf.PerlinNoise(seed,        t) - 0.5f) * 2f;
            float y = (Mathf.PerlinNoise(seed + 17f,  t) - 0.5f) * 2f;
            // Additive offset on top of CameraFollow's world position
            transform.position += new Vector3(x, y, 0f) * magnitude;
        }

        private void Trigger(float mag, float dur)
        {
            if (mag * dur < magnitude * remaining) return;
            magnitude = mag;
            remaining = dur;
        }

        private void OnDamageDealt(DamageDealtEvent _)        => Trigger(hitMagnitude, hitDuration);
        private void OnBossStart(BossEncounterStartedEvent _) => Trigger(bossIntroMagnitude, bossIntroDuration);
        private void OnPlayerDied(PlayerDiedEvent _)          => Trigger(deathMagnitude, deathDuration);
    }
}
