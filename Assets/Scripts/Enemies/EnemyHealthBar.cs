using UnityEngine;
using UnityEngine.UI;

namespace CMGTSA.Enemies
{
    /// <summary>
    /// Per-enemy world-space HP bar. Subscribes to its parent <see cref="EnemyController"/>'s
    /// <c>onHPChanged</c> C# event. Observer pattern, intra-system: bus would force every bar
    /// to filter by transform, which is wrong shape for this signal.
    /// </summary>
    public class EnemyHealthBar : MonoBehaviour
    {
        [SerializeField] private EnemyController controller;
        [SerializeField] private Image fillImage;

        private void Reset()
        {
            controller = GetComponentInParent<EnemyController>();
        }

        private void OnEnable()
        {
            if (controller == null)
            {
                controller = GetComponentInParent<EnemyController>();
            }
            if (controller != null) controller.onHPChanged += OnHPChanged;
        }

        private void OnDisable()
        {
            if (controller != null) controller.onHPChanged -= OnHPChanged;
        }

        private void OnHPChanged(int current, int max)
        {
            if (fillImage == null) return;
            fillImage.fillAmount = max > 0 ? Mathf.Clamp01(current / (float)max) : 0f;
        }
    }
}
