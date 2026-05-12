using UnityEngine;

namespace CMGTSA.Spawner
{
    public interface INavMeshSampler
    {
        bool TrySample(Vector3 source, float maxDistance, out Vector3 valid);
    }
}
