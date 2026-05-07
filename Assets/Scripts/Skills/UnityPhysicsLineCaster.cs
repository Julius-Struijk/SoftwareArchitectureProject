using UnityEngine;

namespace CMGTSA.Skills
{
    public class UnityPhysicsLineCaster : IPhysicsLineCaster
    {
        public bool TryCast(Vector3 from, Vector3 to, LayerMask mask, out Vector2 hitPoint)
        {
            RaycastHit2D hit = Physics2D.Linecast(from, to, mask);
            if (hit.collider != null)
            {
                hitPoint = hit.point;
                return true;
            }
            hitPoint = default;
            return false;
        }
    }
}
