using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

/// <summary>
/// Creates CombatPlayModeScene and adds it to Build Settings for PlayMode tests.
/// </summary>
public class Slice2TestSceneSetup
{
    public static void Execute()
    {
        CreateCombatPlayModeScene();
        Debug.Log("Slice2TestSceneSetup: done.");
    }

    private static void CreateCombatPlayModeScene()
    {
        string dir = "Assets/Scenes/Tests";
        if (!System.IO.Directory.Exists(dir))
            System.IO.Directory.CreateDirectory(dir);

        string scenePath = $"{dir}/CombatPlayModeScene.unity";

        // Create an empty scene.
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
        scene.name = "CombatPlayModeScene";

        // Save it to disk.
        EditorSceneManager.SaveScene(scene, scenePath);

        // Add to Build Settings if not already present.
        var scenes = EditorBuildSettings.scenes;
        bool alreadyInBuild = false;
        foreach (var s in scenes)
        {
            if (s.path == scenePath) { alreadyInBuild = true; break; }
        }

        if (!alreadyInBuild)
        {
            var newScenes = new EditorBuildSettingsScene[scenes.Length + 1];
            System.Array.Copy(scenes, newScenes, scenes.Length);
            newScenes[scenes.Length] = new EditorBuildSettingsScene(scenePath, true);
            EditorBuildSettings.scenes = newScenes;
            Debug.Log($"Added {scenePath} to Build Settings.");
        }
        else
        {
            Debug.Log($"{scenePath} already in Build Settings.");
        }

        // Close the additive scene and return to GameScene.
        EditorSceneManager.CloseScene(scene, true);

        AssetDatabase.Refresh();
        Debug.Log($"Created {scenePath}.");
    }
}
