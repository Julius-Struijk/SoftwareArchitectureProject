using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;

    Vector3 offset;


    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - target.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = target.position;
        transform.position = new Vector3(transform.position.x, transform.position.y, offset.z);
    }

}
