using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using CMGTSA.Boss;
using CMGTSA.UI;

/// <summary>
/// One-shot Editor setup for slice 6. Builds 4 prefabs: Boss, BossProjectile, BossHPBar,
/// and VictoryPanel. Run Slice6PrefabSetup first, then run Slice6Setup to wire the
/// projectile prefab into the ProjectileSpray SO. Run via Coplay execute_script —
/// call <see cref="Execute"/> after a clean compile.
/// </summary>
public class Slice6PrefabSetup
{
    private const string PrefabsDir           = "Assets/Prefabs/Boss";
    private const string BossPrefabPath       = "Assets/Prefabs/Boss/Boss.prefab";
    private const string ProjectilePrefabPath = "Assets/Prefabs/Boss/BossProjectile.prefab";
    private const string BossHPBarPrefabPath  = "Assets/Prefabs/Boss/BossHPBar.prefab";
    private const string VictoryPrefabPath    = "Assets/Prefabs/Boss/VictoryPanel.prefab";
    private const string BossDataPath         = "Assets/Scriptable Objects/Boss/Boss_LichKing.asset";

    public static void Execute()
    {
        EnsureDir(PrefabsDir);
        SaveProjectilePrefab();   // build projectile first so Boss can reference it
        SaveBossPrefab();
        SaveBossHPBarPrefab();
        SaveVictoryPanelPrefab();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Slice6PrefabSetup: 4 prefabs created/updated.");
    }

    private static void SaveBossPrefab()
    {
        GameObject root = new GameObject("Boss");
        Rigidbody2D body = root.AddComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Kinematic;
        CircleCollider2D col = root.AddComponent<CircleCollider2D>();
        col.radius = 0.5f;
        SpriteRenderer sr = root.AddComponent<SpriteRenderer>();
        sr.color = new Color(0.5f, 0.1f, 0.6f, 1f);
        NavMeshAgent agent = root.AddComponent<NavMeshAgent>();
        agent.radius = 0.45f;
        agent.height = 1f;
        agent.acceleration = 12f;
        agent.angularSpeed = 0f;
        agent.speed = 1.8f;

        BossController controller = root.AddComponent<BossController>();
        BossData bossData = AssetDatabase.LoadAssetAtPath<BossData>(BossDataPath);
        if (bossData != null)
        {
            SerializedObject so = new SerializedObject(controller);
            so.FindProperty("bossData").objectReferenceValue = bossData;
            so.FindProperty("agent").objectReferenceValue = agent;
            so.ApplyModifiedPropertiesWithoutUndo();
        }
        else
        {
            Debug.LogWarning("Slice6PrefabSetup: Boss_LichKing.asset missing — run Slice6Setup first.");
        }

        PrefabUtility.SaveAsPrefabAsset(root, BossPrefabPath);
        Object.DestroyImmediate(root);
    }

    private static void SaveProjectilePrefab()
    {
        GameObject root = new GameObject("BossProjectile");
        Rigidbody2D body = root.AddComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Kinematic;
        CircleCollider2D col = root.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.15f;
        SpriteRenderer sr = root.AddComponent<SpriteRenderer>();
        sr.color = new Color(1f, 0.4f, 0.1f, 1f);

        BossProjectile projectile = root.AddComponent<BossProjectile>();
        int playerLayer = LayerMask.NameToLayer("Player");
        if (playerLayer >= 0)
        {
            SerializedObject so = new SerializedObject(projectile);
            so.FindProperty("playerLayer").intValue = 1 << playerLayer;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        PrefabUtility.SaveAsPrefabAsset(root, ProjectilePrefabPath);
        Object.DestroyImmediate(root);
    }

    private static void SaveBossHPBarPrefab()
    {
        GameObject root = new GameObject("BossHPBar");
        Canvas canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 5;
        root.AddComponent<CanvasScaler>();
        root.AddComponent<GraphicRaycaster>();
        CanvasGroup group = root.AddComponent<CanvasGroup>();
        group.alpha = 0f;

        GameObject bg = new GameObject("Background", typeof(RectTransform), typeof(Image));
        bg.transform.SetParent(root.transform, false);
        RectTransform bgRT = bg.GetComponent<RectTransform>();
        bgRT.anchorMin = new Vector2(0.2f, 0.92f);
        bgRT.anchorMax = new Vector2(0.8f, 0.96f);
        bgRT.offsetMin = Vector2.zero;
        bgRT.offsetMax = Vector2.zero;
        bg.GetComponent<Image>().color = new Color(0.15f, 0.05f, 0.05f, 0.9f);

        GameObject fill = new GameObject("Fill", typeof(RectTransform), typeof(Image));
        fill.transform.SetParent(bg.transform, false);
        RectTransform fillRT = fill.GetComponent<RectTransform>();
        fillRT.anchorMin = new Vector2(0f, 0f);
        fillRT.anchorMax = new Vector2(1f, 1f);
        fillRT.offsetMin = new Vector2(2f, 2f);
        fillRT.offsetMax = new Vector2(-2f, -2f);
        Image fillImage = fill.GetComponent<Image>();
        fillImage.color = new Color(0.85f, 0.15f, 0.2f, 1f);

        // BossHPBarPresenter uses UnityEngine.UI.Text (not TMP) for bossNameLabel
        GameObject label = new GameObject("BossName", typeof(RectTransform), typeof(Text));
        label.transform.SetParent(root.transform, false);
        RectTransform labelRT = label.GetComponent<RectTransform>();
        labelRT.anchorMin = new Vector2(0.2f, 0.96f);
        labelRT.anchorMax = new Vector2(0.8f, 1f);
        labelRT.offsetMin = Vector2.zero;
        labelRT.offsetMax = Vector2.zero;
        Text labelText = label.GetComponent<Text>();
        labelText.alignment = TextAnchor.MiddleCenter;
        labelText.color = Color.white;
        labelText.text = "Boss";
        labelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        BossHPBarPresenter presenter = root.AddComponent<BossHPBarPresenter>();
        SerializedObject presenterSO = new SerializedObject(presenter);
        presenterSO.FindProperty("group").objectReferenceValue = group;
        presenterSO.FindProperty("fillImage").objectReferenceValue = fillImage;
        presenterSO.FindProperty("bossNameLabel").objectReferenceValue = labelText;
        presenterSO.ApplyModifiedPropertiesWithoutUndo();

        PrefabUtility.SaveAsPrefabAsset(root, BossHPBarPrefabPath);
        Object.DestroyImmediate(root);
    }

    private static void SaveVictoryPanelPrefab()
    {
        GameObject root = new GameObject("VictoryPanel");
        Canvas canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;
        root.AddComponent<CanvasScaler>();
        root.AddComponent<GraphicRaycaster>();
        CanvasGroup group = root.AddComponent<CanvasGroup>();
        group.alpha = 0f;

        GameObject bg = new GameObject("Background", typeof(RectTransform), typeof(Image));
        bg.transform.SetParent(root.transform, false);
        RectTransform bgRT = bg.GetComponent<RectTransform>();
        bgRT.anchorMin = Vector2.zero;
        bgRT.anchorMax = Vector2.one;
        bgRT.offsetMin = Vector2.zero;
        bgRT.offsetMax = Vector2.zero;
        bg.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.65f);

        GameObject titleGO = new GameObject("Title", typeof(RectTransform), typeof(Text));
        titleGO.transform.SetParent(root.transform, false);
        RectTransform titleRT = titleGO.GetComponent<RectTransform>();
        titleRT.anchorMin = new Vector2(0.2f, 0.55f);
        titleRT.anchorMax = new Vector2(0.8f, 0.75f);
        titleRT.offsetMin = Vector2.zero;
        titleRT.offsetMax = Vector2.zero;
        Text titleText = titleGO.GetComponent<Text>();
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.text = "VICTORY";
        titleText.fontSize = 64;
        titleText.color = new Color(1f, 0.9f, 0.2f, 1f);
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        GameObject hint = new GameObject("Hint", typeof(RectTransform), typeof(Text));
        hint.transform.SetParent(root.transform, false);
        RectTransform hintRT = hint.GetComponent<RectTransform>();
        hintRT.anchorMin = new Vector2(0.2f, 0.4f);
        hintRT.anchorMax = new Vector2(0.8f, 0.5f);
        hintRT.offsetMin = Vector2.zero;
        hintRT.offsetMax = Vector2.zero;
        Text hintText = hint.GetComponent<Text>();
        hintText.alignment = TextAnchor.MiddleCenter;
        hintText.text = "Press R to restart";
        hintText.fontSize = 24;
        hintText.color = Color.white;
        hintText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        VictoryUI ui = root.AddComponent<VictoryUI>();
        SerializedObject uiSO = new SerializedObject(ui);
        uiSO.FindProperty("panelGroup").objectReferenceValue = group;
        uiSO.ApplyModifiedPropertiesWithoutUndo();

        PrefabUtility.SaveAsPrefabAsset(root, VictoryPrefabPath);
        Object.DestroyImmediate(root);
    }

    private static void EnsureDir(string path)
    {
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
    }
}
