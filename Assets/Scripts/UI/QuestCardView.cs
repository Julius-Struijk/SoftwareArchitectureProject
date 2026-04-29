using System.Text;
using TMPro;
using UnityEngine;
using CMGTSA.Quests;

namespace CMGTSA.UI
{
    /// <summary>
    /// One quest card row on the HUD. Owns no subscriptions — the presenter calls
    /// <see cref="Bind"/> after every relevant quest event with a fresh
    /// <see cref="QuestProgress"/> snapshot. <see cref="MarkCompleted"/> swaps the body
    /// text to a single "Complete!" line on <see cref="QuestCompletedEvent"/>.
    /// </summary>
    public class QuestCardView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI titleLabel;
        [SerializeField] private TextMeshProUGUI bodyLabel;

        private static readonly StringBuilder builder = new StringBuilder();

        public void Bind(QuestProgress progress)
        {
            if (progress == null || progress.Data == null) return;
            if (titleLabel != null) titleLabel.text = progress.Data.displayName;
            if (bodyLabel == null) return;

            builder.Clear();
            for (int i = 0; i < progress.Goals.Length; i++)
            {
                if (progress.Goals[i] == null) continue;
                if (i > 0) builder.AppendLine();
                builder.Append("- ");
                builder.Append(progress.Goals[i].Describe());
            }
            bodyLabel.text = builder.ToString();
        }

        public void MarkCompleted()
        {
            if (bodyLabel != null) bodyLabel.text = "Complete!";
        }
    }
}
