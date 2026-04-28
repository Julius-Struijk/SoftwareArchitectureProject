using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary>
/// Points InputSystemUIInputModule at the project's existing InputSystem_Actions.inputactions
/// (which already contains a "UI" map) using SerializedObject so the assignment persists in
/// the scene file. Also wires individual action references so the module works in all IS versions.
/// </summary>
public static class FixUIInputModule
{
    private const string ActionsPath = "Assets/InputSystem_Actions.inputactions";

    public static void Execute()
    {
        var es = Object.FindObjectOfType<EventSystem>(true);
        if (es == null) { Debug.LogError("[FixUIInputModule] No EventSystem in scene."); return; }

        var module = es.GetComponent<InputSystemUIInputModule>();
        if (module == null) { Debug.LogError("[FixUIInputModule] No InputSystemUIInputModule found."); return; }

        var asset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(ActionsPath);
        if (asset == null) { Debug.LogError($"[FixUIInputModule] Cannot load {ActionsPath}"); return; }

        // Load all sub-assets (InputActionReference objects live here).
        var allSubs = AssetDatabase.LoadAllAssetsAtPath(ActionsPath);

        var so = new SerializedObject(module);
        so.Update();

        so.FindProperty("m_ActionsAsset").objectReferenceValue = asset;
        SetRef(so, allSubs, "m_PointAction",       "UI/Point");
        SetRef(so, allSubs, "m_LeftClickAction",   "UI/Click");
        SetRef(so, allSubs, "m_RightClickAction",  "UI/RightClick");
        SetRef(so, allSubs, "m_MiddleClickAction", "UI/MiddleClick");
        SetRef(so, allSubs, "m_ScrollWheelAction", "UI/ScrollWheel");
        SetRef(so, allSubs, "m_MoveAction",        "UI/Navigate");
        SetRef(so, allSubs, "m_SubmitAction",      "UI/Submit");
        SetRef(so, allSubs, "m_CancelAction",      "UI/Cancel");

        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(module);
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();

        Debug.Log($"[FixUIInputModule] Done. actionsAsset={module.actionsAsset}  point={module.point}");
    }

    private static void SetRef(SerializedObject so, Object[] subs, string propName, string actionPath)
    {
        foreach (var s in subs)
        {
            if (s is InputActionReference r && r != null && r.name == actionPath)
            {
                so.FindProperty(propName).objectReferenceValue = r;
                return;
            }
        }
        Debug.LogWarning($"[FixUIInputModule] Sub-asset not found for {propName} ({actionPath}) — skipping.");
    }
}
