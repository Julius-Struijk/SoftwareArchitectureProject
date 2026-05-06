using System.Collections;
using UnityEngine;
using CMGTSA.Core;
using CMGTSA.Player;

namespace CMGTSA.Feel
{
    /// <summary>
    /// Slice-7 polish (Tier 2): flashes the player's <c>SpriteRenderer.color</c> red on
    /// any HP drop. Lives on the player prefab so the renderer reference is local; no
    /// scene wiring required beyond the component.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerHurtFlash : MonoBehaviour
    {
        [SerializeField] private Color flashColor   = new Color(1f, 0.3f, 0.3f);
        [Min(0f)] [SerializeField] private float flashDuration = 0.12f;

        private SpriteRenderer sr;
        private Color baseColor;
        private Coroutine flashRoutine;

        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
            baseColor = sr.color;
        }

        private void OnEnable()
        {
            EventBus<PlayerHPChangedEvent>.Subscribe(OnHPChanged);
        }

        private void OnDisable()
        {
            EventBus<PlayerHPChangedEvent>.Unsubscribe(OnHPChanged);
        }

        private void OnHPChanged(PlayerHPChangedEvent evt)
        {
            if (evt.Delta >= 0) return;
            if (flashRoutine != null) StopCoroutine(flashRoutine);
            flashRoutine = StartCoroutine(Flash());
        }

        private IEnumerator Flash()
        {
            sr.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            sr.color = baseColor;
            flashRoutine = null;
        }
    }
}
