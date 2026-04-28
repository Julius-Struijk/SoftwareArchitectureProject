using System.Collections.Generic;
using UnityEngine;
using CMGTSA.Inventory;
using CMGTSA.Player;

namespace CMGTSA.UI
{
    /// <summary>
    /// MVP presenter for the Inventory. Resolves the player's <see cref="InventoryModel"/>
    /// (the M), subscribes to its <see cref="InventoryModel.OnChanged"/> event (Observer
    /// for intra-system one-to-many — slice 2's HUD presenters used the bus because HP is
    /// also cross-system; inventory UI updates are local-only). Drives the grid View by
    /// pooling <see cref="slotPrefab"/> instances under <see cref="grid"/>.
    /// Sort buttons call <see cref="SetSort"/> with a fresh strategy instance.
    /// </summary>
    public class InventoryPresenter : MonoBehaviour
    {
        [Header("View")]
        [SerializeField] private RectTransform grid;
        [SerializeField] private InventorySlotView slotPrefab;

        [Header("Source")]
        [Tooltip("If null, the presenter looks up the Player tag in OnEnable.")]
        [SerializeField] private PlayerController playerController;

        private InventoryModel model;
        private readonly List<InventorySlotView> pooledViews = new List<InventorySlotView>();

        private void OnEnable()
        {
            ResolveModel();
            if (model != null)
            {
                model.OnChanged += Refresh;
                Refresh();
            }
        }

        private void OnDisable()
        {
            if (model != null) model.OnChanged -= Refresh;
            model = null;
        }

        private void ResolveModel()
        {
            if (playerController == null)
            {
                var go = GameObject.FindGameObjectWithTag("Player");
                if (go != null) playerController = go.GetComponent<PlayerController>();
            }
            model = playerController != null ? playerController.Inventory : null;
        }

        private void Refresh()
        {
            if (grid == null || slotPrefab == null || model == null) return;

            // Grow the pool to match.
            while (pooledViews.Count < model.Slots.Count)
            {
                var view = Instantiate(slotPrefab, grid);
                pooledViews.Add(view);
            }

            for (int i = 0; i < pooledViews.Count; i++)
            {
                if (i < model.Slots.Count)
                {
                    int captured = i;
                    pooledViews[i].gameObject.SetActive(true);
                    pooledViews[i].Bind(model.Slots[i], () => model.UseSlot(captured));
                }
                else
                {
                    pooledViews[i].gameObject.SetActive(false);
                }
            }
        }

        public void SetSortNameAscending()    => SetSort(new NameAscendingSort());
        public void SetSortNameDescending()   => SetSort(new NameDescendingSort());
        public void SetSortAttackHighLow()    => SetSort(new AttackHighLowSort());
        public void SetSortObtainedOrder()    => SetSort(new ObtainedOrderSort());

        private void SetSort(IInventorySortStrategy strategy)
        {
            if (model != null) model.SetSortStrategy(strategy);
        }
    }
}
