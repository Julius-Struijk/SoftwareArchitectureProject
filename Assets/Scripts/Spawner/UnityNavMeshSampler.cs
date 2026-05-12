using UnityEngine;
using UnityEngine.AI;

namespace CMGTSA.Spawner
{
    public sealed class UnityNavMeshSampler : INavMeshSampler
    {
        public bool TrySample(Vector3 source, float maxDistance, out Vector3 valid)
        {
            if (NavMesh.SamplePosition(source, out NavMeshHit hit, maxDistance, NavMesh.AllAreas))
            {
                valid = hit.position;
                return true;
            }
            valid = source;
            return false;
        }
    }
}
