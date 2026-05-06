using UnityEngine;
using CMGTSA.Core;
using CMGTSA.Player;

namespace CMGTSA.Feel
{
    /// <summary>
    /// Slice-7 polish (Tier 2): instantiates a "level up" particle prefab parented to the
    /// player so the burst follows them as they walk. The prefab cleans itself up via
    /// its own <c>Stop Action = Destroy</c> setting.
    /// </summary>
    public class LevelUpVFXController : MonoBehaviour
    {
        [SerializeField] private GameObject levelUpPrefab;

        private void OnEnable()
        {
            EventBus<PlayerLeveledUpEvent>.Subscribe(OnLeveledUp);
        }

        private void OnDisable()
        {
            EventBus<PlayerLeveledUpEvent>.Unsubscribe(OnLeveledUp);
        }

        private void OnLeveledUp(PlayerLeveledUpEvent _)
        {
            if (levelUpPrefab == null) return;
            Instantiate(levelUpPrefab, transform.position, Quaternion.identity, transform);
        }
    }
}
