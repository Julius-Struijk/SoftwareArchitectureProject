using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using CMGTSA.Quests;

/// <summary>
/// Wires slice-4 actors into GameScene. Idempotent.
/// </summary>
public class Slice4SceneSetup
{
    private const string CullQuestPath  = "Assets/Scriptable Objects/Quests/Quest_CullTheGhosts.asset";
    private const string FetchQuestPath = "Assets/Scriptable Objects/Quests/Quest_FindTheOldKey.asset";
    private const string PanelPrefabPath = "Assets/Prefabs/QuestPanel.prefab";

    public static void Execute()
    {
        WireGameSystems();
        WireHUDQuestPanel();
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
        AssetDatabase.SaveAssets();
        Debug.Log("Slice4SceneSetup: GameSystems + HUD wired for quests.");
    }

    private static void WireGameSystems()
    {
        var gs = GameObject.Find("GameSystems");
        if (gs == null) { Debug.LogError("GameSystems not found in scene."); return; }

        var manager = gs.GetComponent<QuestManager>();
        if (manager == null) manager = gs.AddComponent<QuestManager>();

        var cull  = AssetDatabase.LoadAssetAtPath<QuestData>(CullQuestPath);
        var fetch = AssetDatabase.LoadAssetAtPath<QuestData>(FetchQuestPath);
        if (cull == null || fetch == null)
        {
            Debug.LogError("Starter quests missing — run Slice4Setup first.");
            return;
        }

        var so = new SerializedObject(manager);
        var arr = so.FindProperty("startingQuests");
        arr.arraySize = 2;
        arr.GetArrayElementAtIndex(0).objectReferenceValue = cull;
        arr.GetArrayElementAtIndex(1).objectReferenceValue = fetch;
        so.ApplyModifiedProperties();
    }

    private static void WireHUDQuestPanel()
    {
        var hud = GameObject.Find("HUD");
        if (hud == null) { Debug.LogError("HUD not found in scene."); return; }

        var existing = hud.transform.Find("QuestPanel");
        if (existing != null) Object.DestroyImmediate(existing.gameObject);

        var panelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PanelPrefabPath);
        if (panelPrefab == null) { Debug.LogError($"{PanelPrefabPath} not found."); return; }

        var instance = (GameObject)PrefabUtility.InstantiatePrefab(panelPrefab, hud.transform);
        instance.name = "QuestPanel";
    }
}
