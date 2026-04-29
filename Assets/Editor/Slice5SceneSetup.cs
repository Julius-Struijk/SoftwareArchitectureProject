using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using CMGTSA.Skills;

/// <summary>
/// One-shot Editor setup for slice 5. Wires the GameSystems GameObject (adds and configures
/// SkillManager and SkillUseInput components) and instantiates SkillsPanel in the HUD.
/// Run via Coplay execute_script — call <see cref="Execute"/> after a clean compile.
/// </summary>
public class Slice5SceneSetup
{
    private const string SpeedSkillPath  = "Assets/Scriptable Objects/Skills/Skill_SpeedBuff.asset";
    private const string SlamSkillPath   = "Assets/Scriptable Objects/Skills/Skill_AOESlam.asset";
    private const string BlinkSkillPath  = "Assets/Scriptable Objects/Skills/Skill_Blink.asset";
    private const string PanelPrefabPath = "Assets/Prefabs/SkillsPanel.prefab";

    public static void Execute()
    {
        WireGameSystems();
        WireHUDSkillsPanel();
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
        AssetDatabase.SaveAssets();
        Debug.Log("Slice5SceneSetup: GameSystems + HUD wired for skills.");
    }

    private static void WireGameSystems()
    {
        var gs = GameObject.Find("GameSystems");
        if (gs == null) { Debug.LogError("GameSystems not found in scene."); return; }

        var manager = gs.GetComponent<SkillManager>();
        if (manager == null) manager = gs.AddComponent<SkillManager>();

        var input = gs.GetComponent<SkillUseInput>();
        if (input == null) input = gs.AddComponent<SkillUseInput>();

        var speed  = AssetDatabase.LoadAssetAtPath<SkillData>(SpeedSkillPath);
        var slam   = AssetDatabase.LoadAssetAtPath<SkillData>(SlamSkillPath);
        var blink  = AssetDatabase.LoadAssetAtPath<SkillData>(BlinkSkillPath);
        if (speed == null || slam == null || blink == null)
        {
            Debug.LogError("Skill SOs missing — run Slice5Setup first.");
            return;
        }

        var so = new SerializedObject(manager);
        var arr = so.FindProperty("availableSkills");
        arr.arraySize = 3;
        arr.GetArrayElementAtIndex(0).objectReferenceValue = speed;
        arr.GetArrayElementAtIndex(1).objectReferenceValue = slam;
        arr.GetArrayElementAtIndex(2).objectReferenceValue = blink;
        so.FindProperty("initialLevel").intValue = 1;
        so.ApplyModifiedProperties();
    }

    private static void WireHUDSkillsPanel()
    {
        var hud = GameObject.Find("HUD");
        if (hud == null) { Debug.LogError("HUD not found in scene."); return; }

        var existing = hud.transform.Find("SkillsPanel");
        if (existing != null) Object.DestroyImmediate(existing.gameObject);

        var panelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PanelPrefabPath);
        if (panelPrefab == null) { Debug.LogError($"{PanelPrefabPath} not found."); return; }

        var instance = (GameObject)PrefabUtility.InstantiatePrefab(panelPrefab, hud.transform);
        instance.name = "SkillsPanel";
    }
}
