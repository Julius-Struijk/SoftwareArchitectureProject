using UnityEngine;

namespace CMGTSA.Game
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] Transform target;

        Vector3 offset;

        void Start()
        {
            offset = transform.position - target.position;
        }

        void LateUpdate()
        {
            transform.position = target.position;
            transform.position = new Vector3(transform.position.x, transform.position.y, offset.z);
        }
    }
}
