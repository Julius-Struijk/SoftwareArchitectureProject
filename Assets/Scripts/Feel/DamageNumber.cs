using System.Collections;
using TMPro;
using UnityEngine;

namespace CMGTSA.Feel
{
    /// <summary>
    /// Slice-7 polish: the component on a DamageNumber prefab. Spawner calls
    /// <see cref="Show"/>; coroutine animates rise + fade then destroys self.
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    public class DamageNumber : MonoBehaviour
    {
        [Min(0.1f)] [SerializeField] private float lifetimeSeconds = 0.7f;
        [Min(0f)]   [SerializeField] private float riseDistance    = 0.8f;

        public void Show(int amount, Color color)
        {
            var text = GetComponent<TMP_Text>();
            text.text  = amount.ToString(System.Globalization.CultureInfo.InvariantCulture);
            text.color = color;
            StartCoroutine(Animate());
        }

        private IEnumerator Animate()
        {
            var text = GetComponent<TMP_Text>();
            Vector3 start = transform.position;
            Vector3 end   = start + Vector3.up * riseDistance;
            Color c0 = text.color;
            float t  = 0f;

            while (t < lifetimeSeconds)
            {
                t += Time.deltaTime;
                float u = Mathf.Clamp01(t / lifetimeSeconds);
                transform.position = Vector3.Lerp(start, end, u);
                text.color = new Color(c0.r, c0.g, c0.b, 1f - u);
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}
