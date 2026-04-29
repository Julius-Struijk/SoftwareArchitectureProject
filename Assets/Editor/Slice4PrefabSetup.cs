using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using CMGTSA.UI;

/// <summary>
/// Builds slice-4 UI prefabs from code. Idempotent: deletes and rebuilds the prefab
/// each run so designers don't drift from the canonical structure.
/// </summary>
public class Slice4PrefabSetup
{
    private const string PrefabsDir         = "Assets/Prefabs";
    private const string CardPrefabPath     = "Assets/Prefabs/QuestCardView.prefab";
    private const string PanelPrefabPath    = "Assets/Prefabs/QuestPanel.prefab";

    public static void Execute()
    {
        EnsureDir(PrefabsDir);
        BuildCardPrefab();
        BuildPanelPrefab();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Slice4PrefabSetup: QuestCardView + QuestPanel prefabs (re)built.");
    }

    private static void BuildCardPrefab()
    {
        var go = new GameObject("QuestCardView", typeof(RectTransform));
        var rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(280, 56);

        var bg = go.AddComponent<Image>();
        bg.color = new Color(0f, 0f, 0f, 0.55f);

        var vlg = go.AddComponent<VerticalLayoutGroup>();
        vlg.padding = new RectOffset(8, 8, 6, 6);
        vlg.spacing = 2;
        vlg.childAlignment = TextAnchor.UpperLeft;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;

        var fitter = go.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        var titleGO = new GameObject("Title", typeof(RectTransform));
        titleGO.transform.SetParent(go.transform, false);
        var title = titleGO.AddComponent<TextMeshProUGUI>();
        title.text = "Quest Title";
        title.fontSize = 16;
        title.fontStyle = FontStyles.Bold;
        title.color = Color.white;

        var bodyGO = new GameObject("Body", typeof(RectTransform));
        bodyGO.transform.SetParent(go.transform, false);
        var body = bodyGO.AddComponent<TextMeshProUGUI>();
        body.text = "- progress 0 / 1";
        body.fontSize = 14;
        body.color = new Color(0.85f, 0.85f, 0.85f);

        var view = go.AddComponent<QuestCardView>();
        var so = new SerializedObject(view);
        so.FindProperty("titleLabel").objectReferenceValue = title;
        so.FindProperty("bodyLabel").objectReferenceValue = body;
        so.ApplyModifiedProperties();

        if (File.Exists(CardPrefabPath)) AssetDatabase.DeleteAsset(CardPrefabPath);
        PrefabUtility.SaveAsPrefabAsset(go, CardPrefabPath);
        Object.DestroyImmediate(go);
    }

    private static void BuildPanelPrefab()
    {
        var card = AssetDatabase.LoadAssetAtPath<QuestCardView>(CardPrefabPath);
        if (card == null) { Debug.LogError($"{CardPrefabPath} missing — card build failed."); return; }

        var go = new GameObject("QuestPanel", typeof(RectTransform));
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(1f, 1f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot     = new Vector2(1f, 1f);
        rt.anchoredPosition = new Vector2(-16f, -16f);
        rt.sizeDelta = new Vector2(300, 200);

        var container = new GameObject("CardContainer", typeof(RectTransform));
        container.transform.SetParent(go.transform, false);
        var crt = container.GetComponent<RectTransform>();
        crt.anchorMin = new Vector2(0f, 1f);
        crt.anchorMax = new Vector2(1f, 1f);
        crt.pivot     = new Vector2(0.5f, 1f);
        crt.anchoredPosition = Vector2.zero;
        crt.sizeDelta = new Vector2(0, 0);

        var vlg = container.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 6;
        vlg.childAlignment = TextAnchor.UpperRight;
        vlg.childForceExpandWidth = false;
        vlg.childForceExpandHeight = false;
        vlg.childControlWidth = false;
        vlg.childControlHeight = false;

        var fitter = container.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        var presenter = go.AddComponent<QuestUIPresenter>();
        var so = new SerializedObject(presenter);
        so.FindProperty("cardContainer").objectReferenceValue = crt;
        so.FindProperty("cardPrefab").objectReferenceValue = card;
        so.ApplyModifiedProperties();

        if (File.Exists(PanelPrefabPath)) AssetDatabase.DeleteAsset(PanelPrefabPath);
        PrefabUtility.SaveAsPrefabAsset(go, PanelPrefabPath);
        Object.DestroyImmediate(go);
    }

    private static void EnsureDir(string path)
    {
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
    }
}
