using System.Collections;
using UnityEngine;
using CMGTSA.Battle;
using CMGTSA.Core;

namespace CMGTSA.Feel
{
    /// <summary>
    /// Slice-7 polish: brief time freeze on hit. Subscribes to <see cref="DamageDealtEvent"/>
    /// and dips <see cref="Time.timeScale"/> to <see cref="dipScale"/> for
    /// <see cref="dipUnscaledSeconds"/> of unscaled time, then restores it. Self-stacking is
    /// avoided by an in-progress flag — frequent multi-hit AOEs do not multiply the freeze.
    /// </summary>
    public class HitStopController : MonoBehaviour
    {
        [Tooltip("Time.timeScale during the dip. ~0.05 looks visceral without jarring.")]
        [Range(0f, 1f)] [SerializeField] private float dipScale = 0.05f;

        [Tooltip("Real-time seconds the freeze lasts. ~0.06 is the AAA action-game standard.")]
        [Min(0f)] [SerializeField] private float dipUnscaledSeconds = 0.06f;

        private bool dipping;

        private void OnEnable()
        {
            EventBus<DamageDealtEvent>.Subscribe(OnDamageDealt);
        }

        private void OnDisable()
        {
            EventBus<DamageDealtEvent>.Unsubscribe(OnDamageDealt);
        }

        private void OnDamageDealt(DamageDealtEvent _)
        {
            if (dipping) return;
            StartCoroutine(Dip());
        }

        private IEnumerator Dip()
        {
            dipping = true;
            float previous = Time.timeScale;
            Time.timeScale = dipScale;
            yield return new WaitForSecondsRealtime(dipUnscaledSeconds);
            Time.timeScale = previous;
            dipping = false;
        }
    }
}
