using UnityEngine;
using UnityEditor;
using CMGTSA.Player;
using CMGTSA.Battle;

/// <summary>
/// Sets up Player, Ghost, and Golem prefabs for slice 2:
/// - Player: adds PlayerController, sets its fields, sets Layer=Player(6).
/// - Ghost/Golem: sets Layer=Enemy(7) on all children.
/// </summary>
public class Slice2PrefabSetup
{
    private const int PlayerLayer = 6;
    private const int EnemyLayer  = 7;

    public static void Execute()
    {
        SetupPlayerPrefab();
        SetEnemyLayer("Assets/Prefabs/Ghost.prefab");
        SetEnemyLayer("Assets/Prefabs/Golem.prefab");
        AssetDatabase.SaveAssets();
        Debug.Log("Slice2PrefabSetup: done.");
    }

    private static void SetupPlayerPrefab()
    {
        string path = "Assets/Prefabs/Player.prefab";
        GameObject prefabRoot = PrefabUtility.LoadPrefabContents(path);

        // Set layer on all objects in the prefab.
        SetLayerRecursive(prefabRoot, PlayerLayer);

        // Add PlayerController if missing.
        PlayerController pc = prefabRoot.GetComponent<PlayerController>();
        if (pc == null)
        {
            pc = prefabRoot.AddComponent<PlayerController>();
            Debug.Log("Added PlayerController to Player prefab.");
        }

        // Set serialized fields via SerializedObject.
        SerializedObject so = new SerializedObject(pc);

        so.FindProperty("maxHP").intValue          = 10;
        so.FindProperty("startingMoney").intValue  = 0;
        so.FindProperty("moveSpeed").floatValue    = 3f;
        so.FindProperty("attackRange").floatValue  = 1.5f;
        so.FindProperty("attackInterval").floatValue = 0.4f;
        so.FindProperty("hurtDuration").floatValue = 0.3f;

        // Set attackDamage (inline DamageData).
        SerializedProperty dmgProp = so.FindProperty("attackDamage");
        dmgProp.FindPropertyRelative("damage").intValue   = 2;
        dmgProp.FindPropertyRelative("slowDown").floatValue = 0f;
        dmgProp.FindPropertyRelative("slowDownTime").floatValue = 0f;

        // Assign DamageType_Melee SO.
        DamageType meleeDT = AssetDatabase.LoadAssetAtPath<DamageType>(
            "Assets/Scriptable Objects/DamageType_Melee.asset");
        if (meleeDT != null)
            dmgProp.FindPropertyRelative("damageType").objectReferenceValue = meleeDT;
        else
            Debug.LogWarning("DamageType_Melee.asset not found — run Slice2Setup first.");

        so.ApplyModifiedProperties();

        PrefabUtility.SaveAsPrefabAsset(prefabRoot, path);
        PrefabUtility.UnloadPrefabContents(prefabRoot);
        Debug.Log("Player prefab updated: PlayerController added, Layer=Player.");
    }

    private static void SetEnemyLayer(string path)
    {
        GameObject prefabRoot = PrefabUtility.LoadPrefabContents(path);
        SetLayerRecursive(prefabRoot, EnemyLayer);
        PrefabUtility.SaveAsPrefabAsset(prefabRoot, path);
        PrefabUtility.UnloadPrefabContents(prefabRoot);
        Debug.Log($"Set Enemy layer on {path}.");
    }

    private static void SetLayerRecursive(GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform child in go.transform)
            SetLayerRecursive(child.gameObject, layer);
    }
}
