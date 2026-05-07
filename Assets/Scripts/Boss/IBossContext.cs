using UnityEngine;
using CMGTSA.Battle;

namespace CMGTSA.Boss
{
    public interface IBossContext
    {
        Vector3 BossPosition { get; }
        Vector3 PlayerPosition { get; }

        void RequestMeleeAttack(float range, DamageData damage);
        void SpawnProjectile(GameObject prefab, Vector3 origin, Vector2 direction, float speed, DamageData damage);
        void SpawnAdd(GameObject prefab, Vector3 position);

        // Returns true and sets valid to the nearest NavMesh position within maxDistance of source.
        // Returns false and sets valid to source (caller must not rely on valid when return is false).
        bool TrySampleNavMesh(Vector3 source, float maxDistance, out Vector3 valid);
    }
}
