using UnityEngine;
using CMGTSA.Core;

namespace CMGTSA.Enemies
{
    /// <summary>
    /// Owns the global alert timer. When the player enters or stays inside its trigger collider,
    /// the alert is raised (or refreshed) and an <see cref="AlertLevelChangedEvent"/> is published
    /// for every <see cref="EnemyController"/> to react to. After alertDuration seconds without
    /// the player in range, the alert drops back to NORMAL.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class AlertManager : MonoBehaviour
    {
        [SerializeField]
        private float alertDuration = 60f;

        private float alertTime;
        private AlertLevel currentLevel = AlertLevel.NORMAL;

        private void OnEnable()
        {
            currentLevel = AlertLevel.NORMAL;
            alertTime = 0f;
        }

        private void Update()
        {
            if (currentLevel != AlertLevel.ALERT) return;

            alertTime += Time.deltaTime;
            if (alertTime >= alertDuration)
            {
                SetLevel(AlertLevel.NORMAL);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player")) RaiseAlert();
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.CompareTag("Player")) RaiseAlert();
        }

        private void RaiseAlert()
        {
            alertTime = 0f;
            if (currentLevel != AlertLevel.ALERT)
            {
                SetLevel(AlertLevel.ALERT);
            }
        }

        private void SetLevel(AlertLevel level)
        {
            if (level == currentLevel) return;
            currentLevel = level;
            EventBus<AlertLevelChangedEvent>.Publish(new AlertLevelChangedEvent(level));
        }
    }
}
