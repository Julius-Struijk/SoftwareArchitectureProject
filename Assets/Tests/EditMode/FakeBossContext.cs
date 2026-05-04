using System.Collections.Generic;
using UnityEngine;
using CMGTSA.Battle;
using CMGTSA.Boss;

namespace CMGTSA.Tests.EditMode
{
    public class FakeBossContext : IBossContext
    {
        public Vector3 BossPositionField = Vector3.zero;
        public Vector3 PlayerPositionField = new Vector3(3f, 0f, 0f);

        public Vector3 BossPosition => BossPositionField;
        public Vector3 PlayerPosition => PlayerPositionField;

        public readonly List<MeleeCall> MeleeCalls = new List<MeleeCall>();
        public readonly List<ProjectileCall> ProjectileCalls = new List<ProjectileCall>();
        public readonly List<AddCall> AddCalls = new List<AddCall>();

        public void RequestMeleeAttack(float range, DamageData damage)
        {
            MeleeCalls.Add(new MeleeCall { Range = range, Damage = damage });
        }

        public void SpawnProjectile(GameObject prefab, Vector3 origin, Vector2 direction, float speed, DamageData damage)
        {
            ProjectileCalls.Add(new ProjectileCall
            {
                Prefab = prefab,
                Origin = origin,
                Direction = direction,
                Speed = speed,
                Damage = damage,
            });
        }

        public void SpawnAdd(GameObject prefab, Vector3 position)
        {
            AddCalls.Add(new AddCall { Prefab = prefab, Position = position });
        }

        public struct MeleeCall { public float Range; public DamageData Damage; }
        public struct ProjectileCall
        {
            public GameObject Prefab;
            public Vector3 Origin;
            public Vector2 Direction;
            public float Speed;
            public DamageData Damage;
        }
        public struct AddCall { public GameObject Prefab; public Vector3 Position; }
    }
}
