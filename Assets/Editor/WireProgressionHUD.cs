using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using CMGTSA.Player;
using CMGTSA.UI;

public class WireProgressionHUD
{
    public static void Execute()
    {
        // ── 1. Find HUDRoot ──────────────────────────────────────────────────
        var hudRootGO = GameObject.Find("HUD/HUDRoot");
        if (hudRootGO == null)
        {
            Debug.LogError("WireProgressionHUD: could not find HUD/HUDRoot in scene.");
            return;
        }

        // ── 2. Build XP bar ──────────────────────────────────────────────────
        // Background (mirrors HPBar_BG: anchor top-left, 240×24)
        var xpBgGO = new GameObject("XPBar_BG");
        xpBgGO.transform.SetParent(hudRootGO.transform, false);
        var xpBgRT = xpBgGO.AddComponent<RectTransform>();
        xpBgRT.anchorMin = new Vector2(0f, 1f);
        xpBgRT.anchorMax = new Vector2(0f, 1f);
        xpBgRT.pivot = new Vector2(0f, 1f);
        xpBgRT.anchoredPosition = new Vector2(40f, -72f);   // 40 below HP bar (24 + 8 gap)
        xpBgRT.sizeDelta = new Vector2(240f, 24f);
        var xpBgImg = xpBgGO.AddComponent<Image>();
        xpBgImg.color = new Color(0.05f, 0.05f, 0.15f, 0.8f);

        // Fill child (Filled / Horizontal)
        var xpFillGO = new GameObject("XPBar_Fill");
        xpFillGO.transform.SetParent(xpBgGO.transform, false);
        var xpFillRT = xpFillGO.AddComponent<RectTransform>();
        xpFillRT.anchorMin = Vector2.zero;
        xpFillRT.anchorMax = Vector2.one;
        xpFillRT.offsetMin = Vector2.zero;
        xpFillRT.offsetMax = Vector2.zero;
        var xpFillImg = xpFillGO.AddComponent<Image>();
        xpFillImg.color = new Color(0.2f, 0.6f, 1f, 1f);   // blue-ish XP colour
        xpFillImg.type = Image.Type.Filled;
        xpFillImg.fillMethod = Image.FillMethod.Horizontal;
        xpFillImg.fillOrigin = 0;
        xpFillImg.fillAmount = 0f;

        // Presenter on the fill object
        var xpBarPresenter = xpFillGO.AddComponent<HUDXPBarPresenter>();
        // Use SerializedObject to set the private serialized field
        var xpBarSO = new SerializedObject(xpBarPresenter);
        xpBarSO.FindProperty("fillImage").objectReferenceValue = xpFillImg;
        xpBarSO.ApplyModifiedProperties();

        // ── 3. Level text ────────────────────────────────────────────────────
        var lvlGO = new GameObject("LevelText");
        lvlGO.transform.SetParent(hudRootGO.transform, false);
        var lvlRT = lvlGO.AddComponent<RectTransform>();
        lvlRT.anchorMin = new Vector2(0f, 1f);
        lvlRT.anchorMax = new Vector2(0f, 1f);
        lvlRT.pivot = new Vector2(0f, 1f);
        lvlRT.anchoredPosition = new Vector2(40f, -100f);
        lvlRT.sizeDelta = new Vector2(120f, 24f);
        var lvlTMP = lvlGO.AddComponent<TextMeshProUGUI>();
        lvlTMP.text = "Lv. 1";
        lvlTMP.fontSize = 18f;
        lvlTMP.color = Color.white;

        var lvlPresenter = lvlGO.AddComponent<HUDLevelTextPresenter>();
        var lvlSO = new SerializedObject(lvlPresenter);
        lvlSO.FindProperty("label").objectReferenceValue = lvlTMP;
        lvlSO.ApplyModifiedProperties();

        // ── 4. XP number text ────────────────────────────────────────────────
        var xpNumGO = new GameObject("XPNumberText");
        xpNumGO.transform.SetParent(hudRootGO.transform, false);
        var xpNumRT = xpNumGO.AddComponent<RectTransform>();
        xpNumRT.anchorMin = new Vector2(0f, 1f);
        xpNumRT.anchorMax = new Vector2(0f, 1f);
        xpNumRT.pivot = new Vector2(0f, 1f);
        xpNumRT.anchoredPosition = new Vector2(160f, -100f);
        xpNumRT.sizeDelta = new Vector2(120f, 24f);
        var xpNumTMP = xpNumGO.AddComponent<TextMeshProUGUI>();
        xpNumTMP.text = "0 / 5";
        xpNumTMP.fontSize = 18f;
        xpNumTMP.color = Color.white;
        xpNumTMP.alignment = TextAlignmentOptions.Right;

        var xpNumPresenter = xpNumGO.AddComponent<HUDXPNumberPresenter>();
        var xpNumSO = new SerializedObject(xpNumPresenter);
        xpNumSO.FindProperty("label").objectReferenceValue = xpNumTMP;
        xpNumSO.ApplyModifiedProperties();

        // ── 5. Assign LevelUpRewardsTable on Player ──────────────────────────
        var player = GameObject.Find("Player");
        if (player == null)
        {
            Debug.LogError("WireProgressionHUD: could not find 'Player' in scene.");
        }
        else
        {
            var pc = player.GetComponent<PlayerController>();
            if (pc == null)
            {
                Debug.LogError("WireProgressionHUD: Player has no PlayerController.");
            }
            else
            {
                var table = AssetDatabase.LoadAssetAtPath<LevelUpRewardsTable>(
                    "Assets/Scriptable Objects/Player/LevelUpRewardsTable.asset");
                if (table == null)
                {
                    Debug.LogError("WireProgressionHUD: LevelUpRewardsTable.asset not found.");
                }
                else
                {
                    var pcSO = new SerializedObject(pc);
                    pcSO.FindProperty("rewardsTable").objectReferenceValue = table;
                    pcSO.ApplyModifiedProperties();
                    Debug.Log("WireProgressionHUD: assigned LevelUpRewardsTable on Player.");
                }
            }
        }

        // ── 6. Mark scene dirty and save ────────────────────────────────────
        EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

        Debug.Log("WireProgressionHUD: done. XP bar, LevelText, XPNumberText added to HUDRoot.");
    }
}
