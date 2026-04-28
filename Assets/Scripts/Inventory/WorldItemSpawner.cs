using UnityEngine;
using CMGTSA.Core;

namespace CMGTSA.Inventory
{
    /// <summary>
    /// Subscribes to <see cref="ItemDroppedEvent"/> and instantiates <see cref="pickupPrefab"/>
    /// at the drop position, configured with the dropped item. Lives on the <c>GameSystems</c>
    /// scene object.
    /// </summary>
    public class WorldItemSpawner : MonoBehaviour
    {
        [Tooltip("Prefab containing an ItemPickup component. Slice 3: Assets/Prefabs/ItemPickup.prefab.")]
        [SerializeField] private ItemPickup pickupPrefab;

        [Tooltip("Optional parent transform for spawned pickups. Leave null to spawn at scene root.")]
        [SerializeField] private Transform pickupParent;

        private void OnEnable()
        {
            EventBus<ItemDroppedEvent>.Subscribe(OnItemDropped);
        }

        private void OnDisable()
        {
            EventBus<ItemDroppedEvent>.Unsubscribe(OnItemDropped);
        }

        private void OnItemDropped(ItemDroppedEvent evt)
        {
            if (pickupPrefab == null || evt.Item == null) return;
            var instance = Instantiate(pickupPrefab, evt.Position, Quaternion.identity, pickupParent);
            instance.Configure(evt.Item);
        }
    }
}
