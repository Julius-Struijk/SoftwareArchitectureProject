using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using CMGTSA.UI;

/// <summary>
/// One-shot Editor setup for slice 5. Builds two UI prefabs: SkillSlotView (a single skill slot
/// button with icon, cooldown overlay, and key label) and SkillsPanel (a horizontal row of 3
/// SkillSlotView instances). Run via Coplay execute_script — call <see cref="Execute"/> after a clean compile.
/// </summary>
public class Slice5PrefabSetup
{
    private const string PrefabsDir       = "Assets/Prefabs";
    private const string SlotPrefabPath   = "Assets/Prefabs/SkillSlotView.prefab";
    private const string PanelPrefabPath  = "Assets/Prefabs/SkillsPanel.prefab";

    public static void Execute()
    {
        EnsureDir(PrefabsDir);
        BuildSlotPrefab();
        BuildPanelPrefab();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Slice5PrefabSetup: SkillSlotView + SkillsPanel prefabs (re)built.");
    }

    private static void BuildSlotPrefab()
    {
        var go = new GameObject("SkillSlotView", typeof(RectTransform));
        var rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(64, 64);

        var bg = go.AddComponent<Image>();
        bg.color = new Color(0f, 0f, 0f, 0.55f);

        var iconGO = new GameObject("Icon", typeof(RectTransform));
        iconGO.transform.SetParent(go.transform, false);
        var iconRt = iconGO.GetComponent<RectTransform>();
        iconRt.anchorMin = new Vector2(0.05f, 0.05f);
        iconRt.anchorMax = new Vector2(0.95f, 0.95f);
        iconRt.offsetMin = Vector2.zero;
        iconRt.offsetMax = Vector2.zero;
        var icon = iconGO.AddComponent<Image>();
        icon.color = new Color(0.3f, 0.3f, 0.3f, 0.6f);

        var cdGO = new GameObject("CooldownOverlay", typeof(RectTransform));
        cdGO.transform.SetParent(go.transform, false);
        var cdRt = cdGO.GetComponent<RectTransform>();
        cdRt.anchorMin = new Vector2(0.05f, 0.05f);
        cdRt.anchorMax = new Vector2(0.95f, 0.95f);
        cdRt.offsetMin = Vector2.zero;
        cdRt.offsetMax = Vector2.zero;
        var cooldown = cdGO.AddComponent<Image>();
        cooldown.color = new Color(0f, 0f, 0f, 0.7f);
        cooldown.type = Image.Type.Filled;
        cooldown.fillMethod = Image.FillMethod.Radial360;
        cooldown.fillOrigin = (int)Image.Origin360.Top;
        cooldown.fillClockwise = false;
        cooldown.fillAmount = 1f;
        cooldown.raycastTarget = false;

        var keyGO = new GameObject("KeyLabel", typeof(RectTransform));
        keyGO.transform.SetParent(go.transform, false);
        var keyRt = keyGO.GetComponent<RectTransform>();
        keyRt.anchorMin = new Vector2(0f, 0.7f);
        keyRt.anchorMax = new Vector2(0.4f, 1f);
        keyRt.offsetMin = new Vector2(2f, 0f);
        keyRt.offsetMax = new Vector2(0f, -2f);
        var key = keyGO.AddComponent<TextMeshProUGUI>();
        key.text = "1";
        key.fontSize = 14;
        key.fontStyle = FontStyles.Bold;
        key.color = Color.white;
        key.alignment = TextAlignmentOptions.TopLeft;

        var view = go.AddComponent<SkillSlotView>();
        var so = new SerializedObject(view);
        so.FindProperty("iconImage").objectReferenceValue = icon;
        so.FindProperty("cooldownOverlay").objectReferenceValue = cooldown;
        so.FindProperty("keyLabel").objectReferenceValue = key;
        so.ApplyModifiedProperties();

        if (File.Exists(SlotPrefabPath)) AssetDatabase.DeleteAsset(SlotPrefabPath);
        PrefabUtility.SaveAsPrefabAsset(go, SlotPrefabPath);
        Object.DestroyImmediate(go);
    }

    private static void BuildPanelPrefab()
    {
        var slot = AssetDatabase.LoadAssetAtPath<SkillSlotView>(SlotPrefabPath);
        if (slot == null) { Debug.LogError($"{SlotPrefabPath} missing — slot build failed."); return; }

        var go = new GameObject("SkillsPanel", typeof(RectTransform));
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(1f, 0f);
        rt.anchorMax = new Vector2(1f, 0f);
        rt.pivot     = new Vector2(1f, 0f);
        rt.anchoredPosition = new Vector2(-16f, 16f);
        rt.sizeDelta = new Vector2(220, 72);

        var row = new GameObject("Row", typeof(RectTransform));
        row.transform.SetParent(go.transform, false);
        var rowRt = row.GetComponent<RectTransform>();
        rowRt.anchorMin = Vector2.zero;
        rowRt.anchorMax = Vector2.one;
        rowRt.offsetMin = Vector2.zero;
        rowRt.offsetMax = Vector2.zero;

        var hlg = row.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 6;
        hlg.childAlignment = TextAnchor.MiddleRight;
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = false;
        hlg.childControlWidth = false;
        hlg.childControlHeight = false;

        var slot0 = (SkillSlotView)PrefabUtility.InstantiatePrefab(slot, row.transform);
        var slot1 = (SkillSlotView)PrefabUtility.InstantiatePrefab(slot, row.transform);
        var slot2 = (SkillSlotView)PrefabUtility.InstantiatePrefab(slot, row.transform);
        slot0.gameObject.name = "Slot_1";
        slot1.gameObject.name = "Slot_2";
        slot2.gameObject.name = "Slot_3";

        var presenter = go.AddComponent<SkillsUIPresenter>();
        var so = new SerializedObject(presenter);
        var slotsArr = so.FindProperty("slots");
        slotsArr.arraySize = 3;
        slotsArr.GetArrayElementAtIndex(0).objectReferenceValue = slot0;
        slotsArr.GetArrayElementAtIndex(1).objectReferenceValue = slot1;
        slotsArr.GetArrayElementAtIndex(2).objectReferenceValue = slot2;
        so.ApplyModifiedProperties();

        var toggle = go.AddComponent<SkillsPanel>();
        var toggleSo = new SerializedObject(toggle);
        toggleSo.FindProperty("panelRoot").objectReferenceValue = go;
        toggleSo.FindProperty("startVisible").boolValue = true;
        toggleSo.ApplyModifiedProperties();

        if (File.Exists(PanelPrefabPath)) AssetDatabase.DeleteAsset(PanelPrefabPath);
        PrefabUtility.SaveAsPrefabAsset(go, PanelPrefabPath);
        Object.DestroyImmediate(go);
    }

    private static void EnsureDir(string path)
    {
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
    }
}
