using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CMGTSA.Inventory;
using CMGTSA.UI;

/// <summary>
/// Builds slice-3 prefabs: ItemPickup, InventorySlotView, InventoryPanel.
/// Run via Coplay execute_script. Idempotent — overwrites existing prefabs.
/// </summary>
public class Slice3PrefabSetup
{
    public static void Execute()
    {
        BuildItemPickupPrefab();
        var slotPrefab = BuildInventorySlotViewPrefab();
        BuildInventoryPanelPrefab(slotPrefab);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Slice3PrefabSetup: ItemPickup, InventorySlotView, InventoryPanel prefabs created.");
    }

    private static void BuildItemPickupPrefab()
    {
        var go = new GameObject("ItemPickup");
        go.tag = "Untagged";

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 5;

        var col = go.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.4f;

        var pickup = go.AddComponent<ItemPickup>();
        var so = new SerializedObject(pickup);
        so.FindProperty("spriteRenderer").objectReferenceValue = sr;
        so.ApplyModifiedProperties();

        const string path = "Assets/Prefabs/ItemPickup.prefab";
        PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
        Debug.Log($"Wrote {path}");
    }

    private static InventorySlotView BuildInventorySlotViewPrefab()
    {
        var go = new GameObject("InventorySlotView", typeof(RectTransform));
        var rt = (RectTransform)go.transform;
        rt.sizeDelta = new Vector2(72, 72);

        var bg = go.AddComponent<Image>();
        bg.color = new Color(0.15f, 0.15f, 0.2f, 0.85f);
        var btn = go.AddComponent<Button>();
        btn.targetGraphic = bg;

        var borderGO = new GameObject("EquippedBorder", typeof(RectTransform));
        borderGO.transform.SetParent(go.transform, false);
        var borderRT = (RectTransform)borderGO.transform;
        borderRT.anchorMin = Vector2.zero;
        borderRT.anchorMax = Vector2.one;
        borderRT.offsetMin = new Vector2(-2, -2);
        borderRT.offsetMax = new Vector2(2, 2);
        var borderImg = borderGO.AddComponent<Image>();
        borderImg.color = new Color(0.3f, 1f, 0.4f, 1f);
        borderImg.enabled = false;

        var iconGO = new GameObject("Icon", typeof(RectTransform));
        iconGO.transform.SetParent(go.transform, false);
        var iconRT = (RectTransform)iconGO.transform;
        iconRT.anchorMin = new Vector2(0.1f, 0.1f);
        iconRT.anchorMax = new Vector2(0.9f, 0.9f);
        iconRT.offsetMin = Vector2.zero;
        iconRT.offsetMax = Vector2.zero;
        var iconImg = iconGO.AddComponent<Image>();
        iconImg.preserveAspect = true;

        var countGO = new GameObject("Count", typeof(RectTransform));
        countGO.transform.SetParent(go.transform, false);
        var countRT = (RectTransform)countGO.transform;
        countRT.anchorMin = new Vector2(0.5f, 0f);
        countRT.anchorMax = new Vector2(1f, 0.4f);
        countRT.offsetMin = Vector2.zero;
        countRT.offsetMax = Vector2.zero;
        var countText = countGO.AddComponent<TextMeshProUGUI>();
        countText.text = string.Empty;
        countText.fontSize = 22;
        countText.alignment = TextAlignmentOptions.BottomRight;
        countText.color = Color.white;

        var view = go.AddComponent<InventorySlotView>();
        var so = new SerializedObject(view);
        so.FindProperty("iconImage").objectReferenceValue = iconImg;
        so.FindProperty("countLabel").objectReferenceValue = countText;
        so.FindProperty("equippedBorder").objectReferenceValue = borderImg;
        so.FindProperty("button").objectReferenceValue = btn;
        so.ApplyModifiedProperties();

        const string path = "Assets/Prefabs/InventorySlotView.prefab";
        PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
        Debug.Log($"Wrote {path}");
        return AssetDatabase.LoadAssetAtPath<InventorySlotView>(path);
    }

    private static void BuildInventoryPanelPrefab(InventorySlotView slotPrefab)
    {
        var root = new GameObject("InventoryPanel", typeof(RectTransform));
        var rootRT = (RectTransform)root.transform;
        rootRT.anchorMin = new Vector2(0.5f, 0.5f);
        rootRT.anchorMax = new Vector2(0.5f, 0.5f);
        rootRT.pivot = new Vector2(0.5f, 0.5f);
        rootRT.sizeDelta = new Vector2(640, 480);

        var body = new GameObject("Body", typeof(RectTransform));
        body.transform.SetParent(root.transform, false);
        var bodyRT = (RectTransform)body.transform;
        bodyRT.anchorMin = Vector2.zero;
        bodyRT.anchorMax = Vector2.one;
        bodyRT.offsetMin = Vector2.zero;
        bodyRT.offsetMax = Vector2.zero;
        var bodyImg = body.AddComponent<Image>();
        bodyImg.color = new Color(0f, 0f, 0f, 0.85f);

        var title = new GameObject("Title", typeof(RectTransform));
        title.transform.SetParent(body.transform, false);
        var titleRT = (RectTransform)title.transform;
        titleRT.anchorMin = new Vector2(0, 1);
        titleRT.anchorMax = new Vector2(1, 1);
        titleRT.pivot = new Vector2(0.5f, 1);
        titleRT.anchoredPosition = new Vector2(0, -16);
        titleRT.sizeDelta = new Vector2(0, 48);
        var titleText = title.AddComponent<TextMeshProUGUI>();
        titleText.text = "Inventory";
        titleText.fontSize = 36;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.white;

        var sortRow = new GameObject("SortRow", typeof(RectTransform));
        sortRow.transform.SetParent(body.transform, false);
        var sortRT = (RectTransform)sortRow.transform;
        sortRT.anchorMin = new Vector2(0, 1);
        sortRT.anchorMax = new Vector2(1, 1);
        sortRT.pivot = new Vector2(0.5f, 1);
        sortRT.anchoredPosition = new Vector2(0, -64);
        sortRT.sizeDelta = new Vector2(-32, 36);
        var sortLayout = sortRow.AddComponent<HorizontalLayoutGroup>();
        sortLayout.spacing = 8;
        sortLayout.childAlignment = TextAnchor.MiddleCenter;
        sortLayout.childForceExpandWidth = true;
        sortLayout.childForceExpandHeight = true;

        var gridGO = new GameObject("Grid", typeof(RectTransform));
        gridGO.transform.SetParent(body.transform, false);
        var gridRT = (RectTransform)gridGO.transform;
        gridRT.anchorMin = new Vector2(0, 0);
        gridRT.anchorMax = new Vector2(1, 1);
        gridRT.offsetMin = new Vector2(16, 16);
        gridRT.offsetMax = new Vector2(-16, -112);
        var grid = gridGO.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(72, 72);
        grid.spacing = new Vector2(8, 8);
        grid.padding = new RectOffset(8, 8, 8, 8);

        var presenter = root.AddComponent<InventoryPresenter>();
        var pso = new SerializedObject(presenter);
        pso.FindProperty("grid").objectReferenceValue = gridRT;
        pso.FindProperty("slotPrefab").objectReferenceValue = slotPrefab;
        pso.ApplyModifiedProperties();

        var panel = root.AddComponent<InventoryPanel>();
        var panelSo = new SerializedObject(panel);
        panelSo.FindProperty("panelRoot").objectReferenceValue = body;
        panelSo.FindProperty("startVisible").boolValue = false;
        panelSo.ApplyModifiedProperties();

        AddSortButton(sortRow.transform, "A-Z",   presenter, nameof(InventoryPresenter.SetSortNameAscending));
        AddSortButton(sortRow.transform, "Z-A",   presenter, nameof(InventoryPresenter.SetSortNameDescending));
        AddSortButton(sortRow.transform, "ATK",   presenter, nameof(InventoryPresenter.SetSortAttackHighLow));
        AddSortButton(sortRow.transform, "Order", presenter, nameof(InventoryPresenter.SetSortObtainedOrder));

        const string path = "Assets/Prefabs/InventoryPanel.prefab";
        PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
        Debug.Log($"Wrote {path}");
    }

    private static void AddSortButton(Transform parent, string label,
        InventoryPresenter target, string methodName)
    {
        var go = new GameObject(label, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var img = go.AddComponent<Image>();
        img.color = new Color(0.2f, 0.2f, 0.3f, 1f);
        var btn = go.AddComponent<Button>();
        btn.targetGraphic = img;

        var textGO = new GameObject("Text", typeof(RectTransform));
        textGO.transform.SetParent(go.transform, false);
        var textRT = (RectTransform)textGO.transform;
        textRT.anchorMin = Vector2.zero; textRT.anchorMax = Vector2.one;
        textRT.offsetMin = Vector2.zero; textRT.offsetMax = Vector2.zero;
        var text = textGO.AddComponent<TextMeshProUGUI>();
        text.text = label;
        text.fontSize = 18;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;

        // Wire onClick — local UI button, allowed per master design.
        UnityEditor.Events.UnityEventTools.AddPersistentListener(
            btn.onClick,
            (UnityEngine.Events.UnityAction)System.Delegate.CreateDelegate(
                typeof(UnityEngine.Events.UnityAction), target, methodName));
    }
}
