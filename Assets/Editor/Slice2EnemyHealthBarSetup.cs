using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using CMGTSA.Enemies;

/// <summary>
/// Creates EnemyHealthBar.prefab and adds an instance of it to Ghost.prefab and Golem.prefab.
/// </summary>
public class Slice2EnemyHealthBarSetup
{
    public static void Execute()
    {
        BuildEnemyHealthBarPrefab();
        AddHealthBarToEnemyPrefab("Assets/Prefabs/Ghost.prefab");
        AddHealthBarToEnemyPrefab("Assets/Prefabs/Golem.prefab");
        AssetDatabase.SaveAssets();
        Debug.Log("Slice2EnemyHealthBarSetup: done.");
    }

    private static void BuildEnemyHealthBarPrefab()
    {
        // Root empty GameObject.
        var root = new GameObject("EnemyHealthBar");

        // World-Space Canvas.
        var canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.referencePixelsPerUnit = 100;
        root.AddComponent<CanvasScaler>();
        root.AddComponent<GraphicRaycaster>();

        var rootRT = root.GetComponent<RectTransform>();
        rootRT.sizeDelta = new Vector2(1.2f * 100, 0.18f * 100); // 120 x 18 in canvas units
        root.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

        // Background image.
        var bgGO = new GameObject("BG");
        bgGO.transform.SetParent(root.transform, false);
        var bgImg = bgGO.AddComponent<Image>();
        bgImg.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        var bgRT = bgGO.GetComponent<RectTransform>();
        bgRT.anchorMin = Vector2.zero;
        bgRT.anchorMax = Vector2.one;
        bgRT.offsetMin = Vector2.zero;
        bgRT.offsetMax = Vector2.zero;

        // Fill image (child of BG).
        var fillGO = new GameObject("Fill");
        fillGO.transform.SetParent(bgGO.transform, false);
        var fillImg = fillGO.AddComponent<Image>();
        fillImg.color = new Color(0.8f, 0.1f, 0.1f, 1f);
        fillImg.type = Image.Type.Filled;
        fillImg.fillMethod = Image.FillMethod.Horizontal;
        fillImg.fillOrigin = 0;
        fillImg.fillAmount = 1f;
        var fillRT = fillGO.GetComponent<RectTransform>();
        fillRT.anchorMin = Vector2.zero;
        fillRT.anchorMax = Vector2.one;
        fillRT.offsetMin = Vector2.zero;
        fillRT.offsetMax = Vector2.zero;

        // EnemyHealthBar script.
        var bar = root.AddComponent<EnemyHealthBar>();
        var barSO = new SerializedObject(bar);
        barSO.FindProperty("fillImage").objectReferenceValue = fillImg;
        // Leave controller null — Reset() + OnEnable() will find it from parent at attach time.
        barSO.ApplyModifiedProperties();

        // Save prefab.
        string path = "Assets/Prefabs/EnemyHealthBar.prefab";
        PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
        Debug.Log($"Created {path}");
    }

    private static void AddHealthBarToEnemyPrefab(string prefabPath)
    {
        var prefabRoot = PrefabUtility.LoadPrefabContents(prefabPath);

        // Remove any existing EnemyHealthBar child to avoid duplicates.
        var existingBar = prefabRoot.transform.Find("EnemyHealthBar");
        if (existingBar != null)
        {
            Object.DestroyImmediate(existingBar.gameObject);
        }

        // Instantiate EnemyHealthBar prefab as child.
        var barPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/EnemyHealthBar.prefab");
        if (barPrefab == null)
        {
            Debug.LogError("EnemyHealthBar.prefab not found.");
            PrefabUtility.UnloadPrefabContents(prefabRoot);
            return;
        }

        var barInstance = (GameObject)PrefabUtility.InstantiatePrefab(barPrefab, prefabRoot.transform);
        barInstance.transform.localPosition = new Vector3(0f, 0.7f, 0f);

        // Wire the controller field to the root EnemyController.
        var ctrl = prefabRoot.GetComponent<EnemyController>();
        if (ctrl != null)
        {
            var barScript = barInstance.GetComponent<EnemyHealthBar>();
            var barSO = new SerializedObject(barScript);
            barSO.FindProperty("controller").objectReferenceValue = ctrl;
            barSO.ApplyModifiedProperties();
        }

        PrefabUtility.SaveAsPrefabAsset(prefabRoot, prefabPath);
        PrefabUtility.UnloadPrefabContents(prefabRoot);
        Debug.Log($"Added EnemyHealthBar to {prefabPath}.");
    }
}
