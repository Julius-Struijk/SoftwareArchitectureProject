using UnityEngine;
using CMGTSA.Battle;
using CMGTSA.Core;

namespace CMGTSA.Feel
{
    /// <summary>
    /// Slice-7 polish: hit particles. Subscribes to <see cref="DamageDealtEvent"/> and
    /// instantiates the <see cref="DamageType.VFX"/> particle system at the hit position.
    /// The prefab carries <c>Stop Action = Destroy</c> so spawned instances clean up on
    /// their own — no pool needed for slice 7's hit volume.
    /// </summary>
    public class HitVFXSpawner : MonoBehaviour
    {
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
            ParticleSystem prefab = evt.Damage?.damageType?.VFX;
            if (prefab == null) return;

            ParticleSystem ps = Instantiate(prefab, evt.Position, Quaternion.identity);
            ps.Play();
        }
    }
}
