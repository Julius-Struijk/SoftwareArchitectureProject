using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using CMGTSA.Boss;
using CMGTSA.Game;
using CMGTSA.UI;

/// <summary>
/// One-shot Editor setup for slice 6. Places the Boss, BossRoomTrigger, BossHPBar, and
/// VictoryPanel into GameScene. Run via the menu (CMGTSA / Slice 6 / Run SceneSetup) or
/// via Coplay execute_script — call <see cref="Execute"/> after a clean compile and after
/// Slice6Setup + Slice6PrefabSetup have been executed.
/// </summary>
public class Slice6SceneSetup
{
    private const string ScenePath           = "Assets/Scenes/GameScene.unity";
    private const string BossPrefabPath      = "Assets/Prefabs/Boss/Boss.prefab";
    private const string BossHPBarPrefabPath = "Assets/Prefabs/Boss/BossHPBar.prefab";
    private const string VictoryPrefabPath   = "Assets/Prefabs/Boss/VictoryPanel.prefab";

    [MenuItem("CMGTSA/Slice 6/Run SceneSetup")]
    public static void Execute()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

        // Remove any stale boss objects from previous runs
        DestroyByName(scene, "Boss(Clone)");
        DestroyByName(scene, "Boss");
        DestroyByName(scene, "BossRoomTrigger");
        DestroyByName(scene, "BossHPBar(Clone)");
        DestroyByName(scene, "BossHPBar");
        DestroyByName(scene, "VictoryPanel(Clone)");
        DestroyByName(scene, "VictoryPanel");

        // Place Boss
        GameObject bossPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(BossPrefabPath);
        if (bossPrefab == null) { Debug.LogError($"Slice6SceneSetup: {BossPrefabPath} missing — run Slice6PrefabSetup first."); return; }
        GameObject boss = (GameObject)PrefabUtility.InstantiatePrefab(bossPrefab);
        boss.name = "Boss";
        boss.transform.position = new Vector3(8f, 8f, 0f);
        BossController bossController = boss.GetComponent<BossController>();

        // Place BossRoomTrigger
        GameObject trigger = new GameObject("BossRoomTrigger");
        trigger.transform.position = new Vector3(6f, 6f, 0f);
        BoxCollider2D box = trigger.AddComponent<BoxCollider2D>();
        box.isTrigger = true;
        box.size = new Vector2(2f, 2f);
        BossRoomTrigger triggerComp = trigger.AddComponent<BossRoomTrigger>();
        SerializedObject triggerSO = new SerializedObject(triggerComp);
        triggerSO.FindProperty("boss").objectReferenceValue = bossController;
        triggerSO.ApplyModifiedPropertiesWithoutUndo();

        // Find HUD parent for UI prefabs
        Transform hudParent = FindHUDParent(scene);

        // Place BossHPBar
        GameObject hpBarPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(BossHPBarPrefabPath);
        if (hpBarPrefab == null) { Debug.LogError($"Slice6SceneSetup: {BossHPBarPrefabPath} missing."); return; }
        GameObject hpBar = (GameObject)PrefabUtility.InstantiatePrefab(hpBarPrefab);
        hpBar.name = "BossHPBar";
        if (hudParent != null) hpBar.transform.SetParent(hudParent, false);

        // Place VictoryPanel
        GameObject victoryPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(VictoryPrefabPath);
        if (victoryPrefab == null) { Debug.LogError($"Slice6SceneSetup: {VictoryPrefabPath} missing."); return; }
        GameObject victory = (GameObject)PrefabUtility.InstantiatePrefab(victoryPrefab);
        victory.name = "VictoryPanel";
        if (hudParent != null) victory.transform.SetParent(hudParent, false);

        // Wire VictoryUI → GameManager
        VictoryUI victoryUI = victory.GetComponent<VictoryUI>();
        GameManager gameManager = Object.FindFirstObjectByType<GameManager>();
        if (victoryUI != null && gameManager != null)
        {
            SerializedObject so = new SerializedObject(victoryUI);
            so.FindProperty("gameManager").objectReferenceValue = gameManager;
            so.ApplyModifiedPropertiesWithoutUndo();
        }
        else if (gameManager == null)
        {
            Debug.LogWarning("Slice6SceneSetup: GameManager not found in scene — wire VictoryUI.gameManager manually.");
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("Slice6SceneSetup: boss + trigger + HP bar + victory panel placed in GameScene.");
    }

    private static Transform FindHUDParent(Scene scene)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            if (root.name == "HUD") return root.transform;
            Transform hud = root.transform.Find("HUD");
            if (hud != null) return hud;
        }
        // Fallback: first Canvas in the scene
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            if (root.GetComponent<Canvas>() != null) return root.transform;
        }
        return null;
    }

    private static void DestroyByName(Scene scene, string name)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            if (root.name == name)
            {
                Object.DestroyImmediate(root);
                continue;
            }
            Transform child = root.transform.Find(name);
            if (child != null) Object.DestroyImmediate(child.gameObject);
        }
    }
}
