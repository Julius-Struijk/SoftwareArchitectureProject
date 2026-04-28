using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using CMGTSA.Inventory;
using CMGTSA.Player;

/// <summary>
/// Wires slice-3 actors into GameScene + Player.prefab. Idempotent.
/// </summary>
public class Slice3SceneSetup
{
    public static void Execute()
    {
        WireGameSystems();
        WireHUDInventoryPanel();
        WirePlayerPrefab();
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
        AssetDatabase.SaveAssets();
        Debug.Log("Slice3SceneSetup: GameSystems + HUD + Player.prefab wired for inventory.");
    }

    private static void WireGameSystems()
    {
        var gs = GameObject.Find("GameSystems");
        if (gs == null) { Debug.LogError("GameSystems not found in scene."); return; }

        if (gs.GetComponent<LootDropper>() == null)
            gs.AddComponent<LootDropper>();

        var spawner = gs.GetComponent<WorldItemSpawner>();
        if (spawner == null) spawner = gs.AddComponent<WorldItemSpawner>();

        var pickupPrefab = AssetDatabase.LoadAssetAtPath<ItemPickup>("Assets/Prefabs/ItemPickup.prefab");
        if (pickupPrefab == null) { Debug.LogError("ItemPickup.prefab not found — run Slice3PrefabSetup first."); return; }

        var so = new SerializedObject(spawner);
        so.FindProperty("pickupPrefab").objectReferenceValue = pickupPrefab;
        so.ApplyModifiedProperties();
    }

    private static void WireHUDInventoryPanel()
    {
        var hud = GameObject.Find("HUD");
        if (hud == null) { Debug.LogError("HUD not found in scene."); return; }

        var existing = hud.transform.Find("InventoryPanel");
        if (existing != null) Object.DestroyImmediate(existing.gameObject);

        var panelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/InventoryPanel.prefab");
        if (panelPrefab == null) { Debug.LogError("InventoryPanel.prefab not found."); return; }

        var instance = (GameObject)PrefabUtility.InstantiatePrefab(panelPrefab, hud.transform);
        instance.name = "InventoryPanel";
    }

    private static void WirePlayerPrefab()
    {
        const string playerPrefabPath = "Assets/Prefabs/Player.prefab";
        var playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(playerPrefabPath);
        if (playerPrefab == null) { Debug.LogError("Player.prefab not found."); return; }

        var consumable = AssetDatabase.LoadAssetAtPath<ItemCategory>(
            "Assets/Scriptable Objects/Items/Categories/ItemCategory_Consumable.asset");
        var passive    = AssetDatabase.LoadAssetAtPath<ItemCategory>(
            "Assets/Scriptable Objects/Items/Categories/ItemCategory_PassiveEquip.asset");
        var quest      = AssetDatabase.LoadAssetAtPath<ItemCategory>(
            "Assets/Scriptable Objects/Items/Categories/ItemCategory_QuestItem.asset");

        if (consumable == null || passive == null || quest == null)
        {
            Debug.LogError("ItemCategory SOs missing — run Slice3Setup first.");
            return;
        }

        var contents = PrefabUtility.LoadPrefabContents(playerPrefabPath);
        var controller = contents.GetComponent<PlayerController>();
        if (controller == null)
        {
            Debug.LogError("PlayerController missing on Player.prefab.");
            PrefabUtility.UnloadPrefabContents(contents);
            return;
        }

        var so = new SerializedObject(controller);
        so.FindProperty("consumableCategory").objectReferenceValue = consumable;
        so.FindProperty("passiveCategory").objectReferenceValue = passive;
        so.FindProperty("questCategory").objectReferenceValue = quest;
        so.ApplyModifiedProperties();

        PrefabUtility.SaveAsPrefabAsset(contents, playerPrefabPath);
        PrefabUtility.UnloadPrefabContents(contents);
    }
}
