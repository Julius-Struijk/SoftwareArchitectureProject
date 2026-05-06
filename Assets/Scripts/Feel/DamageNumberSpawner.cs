using UnityEngine;
using CMGTSA.Battle;
using CMGTSA.Core;

namespace CMGTSA.Feel
{
    /// <summary>
    /// Slice-7 polish: floating damage numbers. Subscribes to <see cref="DamageDealtEvent"/>
    /// and instantiates one <c>DamageNumber</c> prefab per hit, slightly jittered to avoid
    /// stacking when two hits land on the same frame.
    /// </summary>
    public class DamageNumberSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject damageNumberPrefab;

        [Tooltip("Random world-units offset on x and y so simultaneous hits do not stack.")]
        [SerializeField] private float jitter = 0.25f;

        [Tooltip("Color used for hits *to* the player.")]
        [SerializeField] private Color playerHurtColor = new Color(1f, 0.4f, 0.4f);

        [Tooltip("Color used for hits *to* enemies.")]
        [SerializeField] private Color enemyHurtColor  = Color.white;

        private void OnEnable()
        {
            EventBus<DamageDealtEvent>.Subscribe(OnDamageDealt);
        }

        private void OnDisable()
        {
            EventBus<DamageDealtEvent>.Unsubscribe(OnDamageDealt);
        }

        private void OnDamageDealt(DamageDealtEvent evt)
        {
            if (damageNumberPrefab == null) return;

            Vector3 pos = evt.Position + new Vector3(
                Random.Range(-jitter, jitter),
                Random.Range(0f, jitter),
                0f);

            GameObject go = Instantiate(damageNumberPrefab, pos, Quaternion.identity);
            DamageNumber dn = go.GetComponent<DamageNumber>();
            if (dn == null) { Destroy(go); return; }

            bool playerHit = evt.Target != null && evt.Target.CompareTag("Player");
            dn.Show(evt.Amount, playerHit ? playerHurtColor : enemyHurtColor);
        }
    }
}
