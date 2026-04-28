using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary>
/// Creates a minimal UI InputActionAsset (Point + Click + ScrollWheel + Navigate/Submit/Cancel)
/// and assigns it to the scene's InputSystemUIInputModule so mouse clicks reach UI buttons.
/// </summary>
public static class FixUIInputModule
{
    private const string AssetPath = "Assets/UI_InputActions.inputactions";

    public static void Execute()
    {
        var es = Object.FindObjectOfType<EventSystem>();
        if (es == null) { Debug.LogError("[FixUIInputModule] No EventSystem in scene."); return; }

        var module = es.GetComponent<InputSystemUIInputModule>();
        if (module == null) { Debug.LogError("[FixUIInputModule] No InputSystemUIInputModule found."); return; }

        // Build the asset.
        var asset = ScriptableObject.CreateInstance<InputActionAsset>();
        asset.name = "UI_InputActions";
        var map = asset.AddActionMap("UI");

        map.AddAction("Point",       InputActionType.PassThrough).AddBinding("<Mouse>/position");
        map.AddAction("Click",       InputActionType.PassThrough).AddBinding("<Mouse>/leftButton");
        map.AddAction("MiddleClick", InputActionType.PassThrough).AddBinding("<Mouse>/middleButton");
        map.AddAction("RightClick",  InputActionType.PassThrough).AddBinding("<Mouse>/rightButton");
        map.AddAction("ScrollWheel", InputActionType.PassThrough).AddBinding("<Mouse>/scroll");

        var nav = map.AddAction("Navigate");
        nav.AddCompositeBinding("2DVector")
            .With("Up",    "<Keyboard>/upArrow")
            .With("Down",  "<Keyboard>/downArrow")
            .With("Left",  "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow");

        map.AddAction("Submit").AddBinding("<Keyboard>/enter");
        map.AddAction("Cancel").AddBinding("<Keyboard>/escape");

        // Overwrite any existing version.
        var existing = AssetDatabase.LoadAssetAtPath<InputActionAsset>(AssetPath);
        if (existing != null) AssetDatabase.DeleteAsset(AssetPath);

        AssetDatabase.CreateAsset(asset, AssetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        var saved = AssetDatabase.LoadAssetAtPath<InputActionAsset>(AssetPath);

        // Assign — this triggers InputSystemUIInputModule.ApplyActionsAsset() internally.
        module.actionsAsset = saved;
        EditorUtility.SetDirty(module);

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();

        Debug.Log($"[FixUIInputModule] Assigned {AssetPath} to InputSystemUIInputModule. UI clicks should now work.");
    }
}
