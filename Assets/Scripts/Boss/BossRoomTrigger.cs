using UnityEngine;
using CMGTSA.Core;

namespace CMGTSA.Boss
{
    [RequireComponent(typeof(Collider2D))]
    public class BossRoomTrigger : MonoBehaviour
    {
        [SerializeField] private BossController boss;
        [SerializeField] private string playerTag = "Player";

        private bool fired;

        private void Reset()
        {
            Collider2D col = GetComponent<Collider2D>();
            if (col != null) col.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (fired) return;
            if (!other.CompareTag(playerTag)) return;
            if (boss == null || boss.Data == null) return;
            fired = true;
            EventBus<BossEncounterStartedEvent>.Publish(new BossEncounterStartedEvent(boss.Data));
        }
    }
}
