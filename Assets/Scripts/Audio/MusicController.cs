using System.Collections;
using UnityEngine;
using CMGTSA.Boss;
using CMGTSA.Core;

namespace CMGTSA.Audio
{
    /// <summary>
    /// Slice-7 polish: dungeon BG music + boss cross-fade. BG plays from <c>Start</c> looped.
    /// On <see cref="BossEncounterStartedEvent"/> fades BG out and boss in over
    /// <see cref="crossFadeSeconds"/>; reversed on <see cref="BossEncounterEndedEvent"/>.
    /// </summary>
    public class MusicController : MonoBehaviour
    {
        [SerializeField] private AudioClip bgClip;
        [SerializeField] private AudioClip bossClip;
        [Range(0f, 1f)] [SerializeField] private float bgVolume   = 0.5f;
        [Range(0f, 1f)] [SerializeField] private float bossVolume = 0.7f;
        [Min(0f)] [SerializeField] private float crossFadeSeconds = 1.5f;

        private AudioSource bgSource;
        private AudioSource bossSource;
        private Coroutine fadeRoutine;

        private void Awake()
        {
            bgSource   = MakeSource(bgClip,   bgVolume,   loop: true, playOnAwake: false);
            bossSource = MakeSource(bossClip, 0f,         loop: true, playOnAwake: false);
        }

        private void OnEnable()
        {
            EventBus<BossEncounterStartedEvent>.Subscribe(OnBossStart);
            EventBus<BossEncounterEndedEvent>.Subscribe(OnBossEnd);
        }

        private void OnDisable()
        {
            EventBus<BossEncounterStartedEvent>.Unsubscribe(OnBossStart);
            EventBus<BossEncounterEndedEvent>.Unsubscribe(OnBossEnd);
        }

        private void Start()
        {
            if (bgSource.clip != null) bgSource.Play();
        }

        private AudioSource MakeSource(AudioClip clip, float volume, bool loop, bool playOnAwake)
        {
            var src = gameObject.AddComponent<AudioSource>();
            src.clip        = clip;
            src.volume      = volume;
            src.loop        = loop;
            src.playOnAwake = playOnAwake;
            return src;
        }

        private void OnBossStart(BossEncounterStartedEvent _)
        {
            if (bossSource.clip != null) bossSource.Play();
            if (fadeRoutine != null) StopCoroutine(fadeRoutine);
            fadeRoutine = StartCoroutine(CrossFade(from: bgSource, to: bossSource, toVolume: bossVolume));
        }

        private void OnBossEnd(BossEncounterEndedEvent _)
        {
            if (fadeRoutine != null) StopCoroutine(fadeRoutine);
            fadeRoutine = StartCoroutine(CrossFade(from: bossSource, to: bgSource, toVolume: bgVolume));
        }

        private IEnumerator CrossFade(AudioSource from, AudioSource to, float toVolume)
        {
            float t = 0f;
            float fromStart = from.volume;
            while (t < crossFadeSeconds)
            {
                t += Time.unscaledDeltaTime;
                float u = Mathf.Clamp01(t / crossFadeSeconds);
                from.volume = Mathf.Lerp(fromStart, 0f,      u);
                to.volume   = Mathf.Lerp(0f,        toVolume, u);
                yield return null;
            }
            from.volume = 0f;
            to.volume   = toVolume;
            from.Stop();
        }
    }
}
