using UnityEngine;
using UnityEngine.UI;
using CMGTSA.Core;
using CMGTSA.Boss;

namespace CMGTSA.UI
{
    public class BossHPBarPresenter : MonoBehaviour
    {
        [SerializeField] private CanvasGroup group;
        [SerializeField] private Image fillImage;
        [SerializeField] private Text bossNameLabel;

        private void Awake()
        {
            SetVisible(false);
        }

        private void OnEnable()
        {
            EventBus<BossEncounterStartedEvent>.Subscribe(OnStarted);
            EventBus<BossEncounterEndedEvent>.Subscribe(OnEnded);
            EventBus<BossHPChangedEvent>.Subscribe(OnHPChanged);
        }

        private void OnDisable()
        {
            EventBus<BossEncounterStartedEvent>.Unsubscribe(OnStarted);
            EventBus<BossEncounterEndedEvent>.Unsubscribe(OnEnded);
            EventBus<BossHPChangedEvent>.Unsubscribe(OnHPChanged);
        }

        private void OnStarted(BossEncounterStartedEvent evt)
        {
            SetVisible(true);
            if (bossNameLabel != null && evt.Data != null) bossNameLabel.text = evt.Data.displayName;
        }

        private void OnEnded(BossEncounterEndedEvent _)
        {
            SetVisible(false);
        }

        private void OnHPChanged(BossHPChangedEvent evt)
        {
            if (fillImage == null) return;
            float fill = evt.Max > 0 ? Mathf.Clamp01(evt.Current / (float)evt.Max) : 0f;
            RectTransform rt = fillImage.rectTransform;
            rt.anchorMax = new Vector2(fill, rt.anchorMax.y);
        }

        private void SetVisible(bool show)
        {
            if (group == null) return;
            group.alpha = show ? 1f : 0f;
            group.interactable = show;
            group.blocksRaycasts = show;
        }
    }
}
