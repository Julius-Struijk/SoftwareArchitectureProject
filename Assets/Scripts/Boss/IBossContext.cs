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
    }
}
