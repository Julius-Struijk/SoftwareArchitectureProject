using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using CMGTSA.Battle;
using CMGTSA.Game;

/// <summary>
/// Wires the scene-level slice-2 objects:
/// - Creates GameSystems GO with DamageResolver (masks set) and GameManager.
/// - Adds the Attack persistent call to the PlayerInput on InputManager.
/// - Updates scene instances of Player/Ghost/Golem from their updated prefabs.
/// </summary>
public class Slice2SceneSetup
{
    private const int PlayerLayer = 6;
    private const int EnemyLayer  = 7;

    public static void Execute()
    {
        AddGameSystems();
        WireAttackAction();
        UpdateSceneInstanceLayers();
        EditorUtility.SetDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects()[0]);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
        Debug.Log("Slice2SceneSetup: done.");
    }

    private static void AddGameSystems()
    {
        // Skip if already present.
        if (GameObject.Find("GameSystems") != null)
        {
            Debug.Log("GameSystems already exists — skipping creation.");
            EnsureGameSystemsComponents();
            return;
        }

        var go = new GameObject("GameSystems");
        go.transform.position = Vector3.zero;

        // DamageResolver
        var resolver = go.AddComponent<DamageResolver>();
        SetLayerMask(resolver, "enemyMask", EnemyLayer);
        SetLayerMask(resolver, "playerMask", PlayerLayer);

        // GameManager
        go.AddComponent<GameManager>();

        Debug.Log("Created GameSystems with DamageResolver + GameManager.");
    }

    private static void EnsureGameSystemsComponents()
    {
        var go = GameObject.Find("GameSystems");
        if (go.GetComponent<DamageResolver>() == null)
        {
            var resolver = go.AddComponent<DamageResolver>();
            SetLayerMask(resolver, "enemyMask", EnemyLayer);
            SetLayerMask(resolver, "playerMask", PlayerLayer);
            Debug.Log("Added DamageResolver to existing GameSystems.");
        }
        if (go.GetComponent<GameManager>() == null)
        {
            go.AddComponent<GameManager>();
            Debug.Log("Added GameManager to existing GameSystems.");
        }
    }

    private static void SetLayerMask(MonoBehaviour target, string fieldName, int layer)
    {
        var f = target.GetType().GetField(fieldName,
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        if (f != null)
        {
            LayerMask mask = 1 << layer;
            f.SetValue(target, mask);
        }
        else
        {
            Debug.LogWarning($"Field {fieldName} not found on {target.GetType().Name}");
        }
    }

    private static void WireAttackAction()
    {
        var inputManagerGO = GameObject.Find("InputManager");
        if (inputManagerGO == null)
        {
            Debug.LogError("InputManager not found in scene.");
            return;
        }

        var playerInput = inputManagerGO.GetComponent<PlayerInput>();
        if (playerInput == null)
        {
            Debug.LogError("PlayerInput component not found on InputManager.");
            return;
        }

        // Find the Player GO in the scene.
        var playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO == null)
        {
            Debug.LogError("No GameObject tagged 'Player' found.");
            return;
        }

        var playerControl = playerGO.GetComponent<CMGTSA.Player.PlayerControl>();
        if (playerControl == null)
        {
            Debug.LogError("PlayerControl not found on Player.");
            return;
        }

        // Check existing events for the Attack action.
        foreach (var actionEvent in playerInput.actionEvents)
        {
            if (actionEvent.actionName.Contains("Attack"))
            {
                // Check if already wired.
                int count = actionEvent.GetPersistentEventCount();
                bool alreadyWired = false;
                for (int i = 0; i < count; i++)
                {
                    if (actionEvent.GetPersistentTarget(i) == playerControl)
                    {
                        alreadyWired = true;
                        break;
                    }
                }

                if (!alreadyWired)
                {
                    UnityEditor.Events.UnityEventTools.AddPersistentListener(
                        actionEvent,
                        new UnityEngine.Events.UnityAction<InputAction.CallbackContext>(
                            playerControl.Attack));
                    EditorUtility.SetDirty(playerInput);
                    Debug.Log("Wired PlayerControl.Attack to PlayerInput Attack action.");
                }
                else
                {
                    Debug.Log("Attack action already wired.");
                }
                return;
            }
        }

        Debug.LogWarning("Attack action event not found in PlayerInput. May need manual wiring.");
    }

    private static void UpdateSceneInstanceLayers()
    {
        // Update Player instance layer.
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            SetLayerRecursive(player, PlayerLayer);
            Debug.Log("Updated Player scene instance layer.");
        }

        // Update all enemy instances by component (no Enemy tag required).
        var controllers = GameObject.FindObjectsByType<CMGTSA.Enemies.EnemyController>(FindObjectsSortMode.None);
        foreach (var ctrl in controllers)
            SetLayerRecursive(ctrl.gameObject, EnemyLayer);
        Debug.Log($"Updated {controllers.Length} enemy instance(s) layer to Enemy.");
    }

    private static void SetLayerRecursive(GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform child in go.transform)
            SetLayerRecursive(child.gameObject, layer);
    }
}
