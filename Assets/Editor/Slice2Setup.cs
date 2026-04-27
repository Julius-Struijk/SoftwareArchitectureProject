using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// One-shot editor setup script for slice 2. Run via Coplay execute_script.
/// Creates DamageType SOs, sets attackDamage on Ghost/Golem, adds Player+Enemy layers.
/// </summary>
public class Slice2Setup
{
    public static void AddLayers()
    {
        SerializedObject tagManager = new SerializedObject(
            AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

        SerializedProperty layers = tagManager.FindProperty("layers");

        // Find first free slots for Player (6) and Enemy (7).
        for (int i = 6; i <= 7; i++)
        {
            SerializedProperty sp = layers.GetArrayElementAtIndex(i);
            if (string.IsNullOrEmpty(sp.stringValue))
            {
                sp.stringValue = i == 6 ? "Player" : "Enemy";
                Debug.Log($"Set layer {i} = {sp.stringValue}");
            }
            else
            {
                Debug.Log($"Layer {i} already set to '{sp.stringValue}' — skipping.");
            }
        }

        tagManager.ApplyModifiedProperties();
        AssetDatabase.SaveAssets();
    }

    public static void CreateDamageTypeSOs()
    {
        string dir = "Assets/Scriptable Objects";
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        CreateSOIfMissing(dir, "DamageType_Melee");
        CreateSOIfMissing(dir, "DamageType_EnemyMelee");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("DamageType SOs created/verified.");
    }

    private static void CreateSOIfMissing(string dir, string name)
    {
        string path = $"{dir}/{name}.asset";
        if (AssetDatabase.LoadAssetAtPath<CMGTSA.Battle.DamageType>(path) != null)
        {
            Debug.Log($"{name} already exists.");
            return;
        }
        var so = ScriptableObject.CreateInstance<CMGTSA.Battle.DamageType>();
        AssetDatabase.CreateAsset(so, path);
        Debug.Log($"Created {path}");
    }

    public static void SetEnemyAttackDamage()
    {
        var meleeSO = AssetDatabase.LoadAssetAtPath<CMGTSA.Battle.DamageType>(
            "Assets/Scriptable Objects/DamageType_EnemyMelee.asset");
        if (meleeSO == null)
        {
            Debug.LogError("DamageType_EnemyMelee.asset not found. Run CreateDamageTypeSOs first.");
            return;
        }

        SetDamageOnEnemy("Assets/Scriptable Objects/Ghost.asset", 1, meleeSO);
        SetDamageOnEnemy("Assets/Scriptable Objects/Golem.asset", 2, meleeSO);

        AssetDatabase.SaveAssets();
        Debug.Log("Enemy attackDamage fields set.");
    }

    private static void SetDamageOnEnemy(string path, int dmgAmount, CMGTSA.Battle.DamageType dmgType)
    {
        var data = AssetDatabase.LoadAssetAtPath<CMGTSA.Enemies.EnemyData>(path);
        if (data == null) { Debug.LogError($"{path} not found."); return; }

        if (data.attackDamage == null)
            data.attackDamage = new CMGTSA.Battle.DamageData();

        data.attackDamage.damage = dmgAmount;
        data.attackDamage.damageType = dmgType;
        EditorUtility.SetDirty(data);
        Debug.Log($"Set {path} attackDamage.damage = {dmgAmount}");
    }

    public static void Execute()
    {
        AddLayers();
        CreateDamageTypeSOs();
        SetEnemyAttackDamage();
    }
}
