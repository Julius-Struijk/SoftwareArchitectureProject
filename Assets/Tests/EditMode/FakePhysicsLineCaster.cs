using System.Collections.Generic;
using UnityEngine;
using CMGTSA.Skills;

namespace CMGTSA.Tests
{
    public class FakePhysicsLineCaster : IPhysicsLineCaster
    {
        public bool ShouldHit;
        public Vector2 HitPoint;

        public struct CastCall
        {
            public Vector3 From;
            public Vector3 To;
            public int MaskValue;
        }

        public readonly List<CastCall> Calls = new List<CastCall>();

        public bool TryCast(Vector3 from, Vector3 to, LayerMask mask, out Vector2 hitPoint)
        {
            Calls.Add(new CastCall { From = from, To = to, MaskValue = mask.value });
            if (ShouldHit)
            {
                hitPoint = HitPoint;
                return true;
            }
            hitPoint = default;
            return false;
        }
    }
}
