using System.Collections.Generic;
using UnityEngine;
using CMGTSA.Core;
using CMGTSA.Quests;

namespace CMGTSA.UI
{
    /// <summary>
    /// MVP presenter for quests. Subscribes to <see cref="QuestStartedEvent"/>,
    /// <see cref="QuestProgressEvent"/>, <see cref="QuestCompletedEvent"/> and drives
    /// a vertical list of <see cref="QuestCardView"/>. Cards are keyed by <see cref="QuestData"/>
    /// reference so the same SO always maps to the same view, even after a scene reload —
    /// the active list is rebuilt by <c>QuestManager</c> on <c>GameRestartedEvent</c>; this
    /// presenter clears its dictionary in <see cref="OnEnable"/>.
    /// </summary>
    public class QuestUIPresenter : MonoBehaviour
    {
        [Header("View")]
        [SerializeField] private RectTransform cardContainer;
        [SerializeField] private QuestCardView cardPrefab;

        private readonly Dictionary<QuestData, QuestCardView> cards = new Dictionary<QuestData, QuestCardView>();

        private void OnEnable()
        {
            ClearCards();
            EventBus<QuestStartedEvent>.Subscribe(OnQuestStarted);
            EventBus<QuestProgressEvent>.Subscribe(OnQuestProgress);
            EventBus<QuestCompletedEvent>.Subscribe(OnQuestCompleted);
        }

        private void OnDisable()
        {
            EventBus<QuestStartedEvent>.Unsubscribe(OnQuestStarted);
            EventBus<QuestProgressEvent>.Unsubscribe(OnQuestProgress);
            EventBus<QuestCompletedEvent>.Unsubscribe(OnQuestCompleted);
        }

        private void ClearCards()
        {
            foreach (var view in cards.Values)
            {
                if (view != null) Destroy(view.gameObject);
            }
            cards.Clear();
        }

        private void OnQuestStarted(QuestStartedEvent evt)
        {
            if (cardContainer == null || cardPrefab == null || evt.Quest == null) return;
            if (!cards.TryGetValue(evt.Quest, out var card) || card == null)
            {
                card = Instantiate(cardPrefab, cardContainer);
                cards[evt.Quest] = card;
            }
            card.Bind(evt.Progress);
        }

        private void OnQuestProgress(QuestProgressEvent evt)
        {
            if (evt.Quest == null) return;
            if (cards.TryGetValue(evt.Quest, out var card) && card != null)
            {
                card.Bind(evt.Progress);
            }
        }

        private void OnQuestCompleted(QuestCompletedEvent evt)
        {
            if (evt.Quest == null) return;
            if (cards.TryGetValue(evt.Quest, out var card) && card != null)
            {
                card.MarkCompleted();
            }
        }
    }
}
