using UnityEngine;
using CMGTSA.Core;

namespace CMGTSA.Inventory
{
    /// <summary>
    /// World pickup. <see cref="WorldItemSpawner"/> sets <see cref="itemData"/> right after
    /// instantiation; this script copies the icon onto the SpriteRenderer in <c>Start</c> so
    /// late-set data still shows. On overlap with a Player-tagged collider, publishes
    /// <see cref="ItemPickedUpEvent"/> and destroys self.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
    public class ItemPickup : MonoBehaviour
    {
        [SerializeField] private ItemData itemData;
        [SerializeField] private SpriteRenderer spriteRenderer;

        public void Configure(ItemData data)
        {
            itemData = data;
            ApplyIcon();
        }

        private void Awake()
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            // The collider in the prefab is set as a trigger — if a designer forgets, fix it here.
            var col = GetComponent<Collider2D>();
            if (col != null && !col.isTrigger) col.isTrigger = true;
        }

        private void Start()
        {
            ApplyIcon();
        }

        private void ApplyIcon()
        {
            if (spriteRenderer != null && itemData != null && itemData.icon != null)
            {
                spriteRenderer.sprite = itemData.icon;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (itemData == null) return;
            if (!other.CompareTag("Player")) return;

            EventBus<ItemPickedUpEvent>.Publish(new ItemPickedUpEvent(itemData, transform.position));
            Destroy(gameObject);
        }
    }
}
