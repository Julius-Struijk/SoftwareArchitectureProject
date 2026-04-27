using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using CMGTSA.UI;
using CMGTSA.Game;

/// <summary>
/// Creates HUD.prefab and GameOverPanel.prefab for slice 2, then adds HUD to the scene.
/// </summary>
public class Slice2HUDSetup
{
    public static void Execute()
    {
        BuildAndSaveHUD();
        BuildAndSaveGameOverPanel();
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
        Debug.Log("Slice2HUDSetup: done.");
    }

    private static void BuildAndSaveHUD()
    {
        // Remove any existing HUD canvas from scene before recreating.
        var existingHUD = GameObject.Find("HUD");
        if (existingHUD != null)
        {
            Object.DestroyImmediate(existingHUD);
        }

        // Create the Canvas.
        var hudGO = new GameObject("HUD");
        var canvas = hudGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = hudGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        hudGO.AddComponent<GraphicRaycaster>();

        // HUDController.
        var hudCtrl = hudGO.AddComponent<HUDController>();

        // HUDRoot child.
        var hudRoot = new GameObject("HUDRoot");
        hudRoot.transform.SetParent(hudGO.transform, false);
        var rootRT = hudRoot.AddComponent<RectTransform>();
        rootRT.anchorMin = Vector2.zero;
        rootRT.anchorMax = Vector2.one;
        rootRT.offsetMin = Vector2.zero;
        rootRT.offsetMax = Vector2.zero;

        // Set HUDController.hudRoot field.
        var so = new SerializedObject(hudCtrl);
        so.FindProperty("hudRoot").objectReferenceValue = hudRoot;
        so.ApplyModifiedProperties();

        // HPBar_BG (background image).
        var bgGO = new GameObject("HPBar_BG");
        bgGO.transform.SetParent(hudRoot.transform, false);
        var bgImg = bgGO.AddComponent<Image>();
        bgImg.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        var bgRT = bgGO.GetComponent<RectTransform>();
        bgRT.anchorMin = new Vector2(0, 1);
        bgRT.anchorMax = new Vector2(0, 1);
        bgRT.pivot = new Vector2(0, 1);
        bgRT.anchoredPosition = new Vector2(40, -40);
        bgRT.sizeDelta = new Vector2(240, 24);

        // HPBar_Fill (fill image inside background).
        var fillGO = new GameObject("HPBar_Fill");
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

        // HUDHPBarPresenter on fill image.
        var barPresenter = fillGO.AddComponent<HUDHPBarPresenter>();
        var barSO = new SerializedObject(barPresenter);
        barSO.FindProperty("fillImage").objectReferenceValue = fillImg;
        barSO.ApplyModifiedProperties();

        // HPNumber (TMP text).
        var numGO = new GameObject("HPNumber");
        numGO.transform.SetParent(hudRoot.transform, false);
        var numText = numGO.AddComponent<TextMeshProUGUI>();
        numText.text = "10 / 10";
        numText.fontSize = 28;
        numText.alignment = TextAlignmentOptions.Left;
        numText.color = Color.white;
        var numRT = numGO.GetComponent<RectTransform>();
        numRT.anchorMin = new Vector2(0, 1);
        numRT.anchorMax = new Vector2(0, 1);
        numRT.pivot = new Vector2(0, 1);
        numRT.anchoredPosition = new Vector2(40, -72);
        numRT.sizeDelta = new Vector2(240, 32);

        // HUDHPNumberPresenter on HP number label.
        var numPresenter = numGO.AddComponent<HUDHPNumberPresenter>();
        var numSO = new SerializedObject(numPresenter);
        numSO.FindProperty("label").objectReferenceValue = numText;
        numSO.ApplyModifiedProperties();

        // Save as prefab.
        EnsureDir("Assets/Prefabs");
        string prefabPath = "Assets/Prefabs/HUD.prefab";
        PrefabUtility.SaveAsPrefabAssetAndConnect(hudGO, prefabPath, InteractionMode.AutomatedAction);
        Debug.Log($"Created HUD prefab at {prefabPath}.");
    }

    private static void BuildAndSaveGameOverPanel()
    {
        // The GameOverPanel is built as a child of the HUD canvas already in the scene.
        var hudGO = GameObject.Find("HUD");
        if (hudGO == null)
        {
            Debug.LogError("HUD not found in scene — run BuildAndSaveHUD first.");
            return;
        }

        // Remove existing panel if present.
        var existing = hudGO.transform.Find("GameOverPanel");
        if (existing != null) Object.DestroyImmediate(existing.gameObject);

        // Create the panel.
        var panelGO = new GameObject("GameOverPanel");
        panelGO.transform.SetParent(hudGO.transform, false);
        var panelImg = panelGO.AddComponent<Image>();
        panelImg.color = new Color(0f, 0f, 0f, 0.7f);
        var panelRT = panelGO.GetComponent<RectTransform>();
        panelRT.anchorMin = Vector2.zero;
        panelRT.anchorMax = Vector2.one;
        panelRT.offsetMin = Vector2.zero;
        panelRT.offsetMax = Vector2.zero;

        // "YOU DIED" label.
        var titleGO = new GameObject("TitleText");
        titleGO.transform.SetParent(panelGO.transform, false);
        var titleText = titleGO.AddComponent<TextMeshProUGUI>();
        titleText.text = "YOU DIED";
        titleText.fontSize = 64;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.white;
        var titleRT = titleGO.GetComponent<RectTransform>();
        titleRT.anchorMin = new Vector2(0.5f, 0.5f);
        titleRT.anchorMax = new Vector2(0.5f, 0.5f);
        titleRT.pivot = new Vector2(0.5f, 0.5f);
        titleRT.anchoredPosition = new Vector2(0, 0);
        titleRT.sizeDelta = new Vector2(600, 100);

        // "Press R to retry" hint.
        var hintGO = new GameObject("HintText");
        hintGO.transform.SetParent(panelGO.transform, false);
        var hintText = hintGO.AddComponent<TextMeshProUGUI>();
        hintText.text = "Press R to retry";
        hintText.fontSize = 28;
        hintText.alignment = TextAlignmentOptions.Center;
        hintText.color = Color.white;
        var hintRT = hintGO.GetComponent<RectTransform>();
        hintRT.anchorMin = new Vector2(0.5f, 0.5f);
        hintRT.anchorMax = new Vector2(0.5f, 0.5f);
        hintRT.pivot = new Vector2(0.5f, 0.5f);
        hintRT.anchoredPosition = new Vector2(0, -90);
        hintRT.sizeDelta = new Vector2(400, 50);

        // GameOverUI component.
        var gameOverUI = panelGO.AddComponent<GameOverUI>();
        var gameSystems = GameObject.Find("GameSystems");
        var gameManager = gameSystems != null ? gameSystems.GetComponent<GameManager>() : null;

        var uiSO = new SerializedObject(gameOverUI);
        uiSO.FindProperty("panelRoot").objectReferenceValue = panelGO;
        if (gameManager != null)
            uiSO.FindProperty("gameManager").objectReferenceValue = gameManager;
        uiSO.ApplyModifiedProperties();

        // Save as prefab.
        string prefabPath = "Assets/Prefabs/GameOverPanel.prefab";
        PrefabUtility.SaveAsPrefabAssetAndConnect(panelGO, prefabPath, InteractionMode.AutomatedAction);
        Debug.Log($"Created GameOverPanel prefab at {prefabPath}.");
    }

    private static void EnsureDir(string path)
    {
        if (!System.IO.Directory.Exists(path))
            System.IO.Directory.CreateDirectory(path);
    }
}
