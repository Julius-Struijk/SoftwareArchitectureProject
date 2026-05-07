using UnityEngine;

namespace CMGTSA.Skills
{
    public interface IPhysicsLineCaster
    {
        bool TryCast(Vector3 from, Vector3 to, LayerMask mask, out Vector2 hitPoint);
    }
}
