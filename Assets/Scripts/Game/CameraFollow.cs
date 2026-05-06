using UnityEngine;
using CMGTSA.Boss;
using CMGTSA.Core;

namespace CMGTSA.Game
{
    [RequireComponent(typeof(Camera))]
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] Transform target;

        [SerializeField, Range(0.1f, 1f)] private float bossEncounterZoomMultiplier = 0.7f;

        Vector3 offset;
        private Camera cam;
        private float baseOrthoSize;
        [SerializeField, Min(0f), Tooltip("SmoothDamp time. ~0.08 feels responsive without jitter.")]
        private float smoothTime = 0.08f;
        private Vector3 followVelocity;

        void Awake()
        {
            cam = GetComponent<Camera>();
            if (cam != null) baseOrthoSize = cam.orthographicSize;
        }

        void Start()
        {
            offset = transform.position - target.position;
        }

        private void OnEnable()
        {
            EventBus<BossEncounterStartedEvent>.Subscribe(OnEncounterStarted);
            EventBus<BossEncounterEndedEvent>.Subscribe(OnEncounterEnded);
        }

        private void OnDisable()
        {
            EventBus<BossEncounterStartedEvent>.Unsubscribe(OnEncounterStarted);
            EventBus<BossEncounterEndedEvent>.Unsubscribe(OnEncounterEnded);
        }

        void LateUpdate()
        {
            if (target == null) return;
            Vector3 desired = new Vector3(target.position.x, target.position.y, transform.position.z);
            Vector3 newPos  = Vector3.SmoothDamp(transform.position, desired, ref followVelocity, smoothTime);
            transform.position = new Vector3(newPos.x, newPos.y, offset.z);
        }

        private void OnEncounterStarted(BossEncounterStartedEvent _)
        {
            if (cam != null) cam.orthographicSize = baseOrthoSize * bossEncounterZoomMultiplier;
        }

        private void OnEncounterEnded(BossEncounterEndedEvent _)
        {
            if (cam != null) cam.orthographicSize = baseOrthoSize;
        }
    }
}
