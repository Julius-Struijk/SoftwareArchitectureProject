using UnityEditor;
using UnityEngine;
using CMGTSA.Battle;

/// <summary>
/// Idempotent: assigns HitParticles.prefab's ParticleSystem to existing DamageType SOs.
/// </summary>
public static class Slice7DamageTypeSetup
{
    private const string MeleeSO       = "Assets/Scriptable Objects/DamageType_Melee.asset";
    private const string EnemyMeleeSO  = "Assets/Scriptable Objects/DamageType_EnemyMelee.asset";
    private const string HitPrefabPath = "Assets/Prefabs/Feel/HitParticles.prefab";

    [MenuItem("CMGTSA/Slice 7/Wire DamageType VFX")]
    public static void Execute()
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(HitPrefabPath);
        if (prefab == null) { Debug.LogError($"Missing {HitPrefabPath} — run Slice7PrefabSetup first."); return; }
        var ps = prefab.GetComponent<ParticleSystem>();
        if (ps == null) { Debug.LogError("HitParticles prefab is missing ParticleSystem."); return; }

        Assign(MeleeSO, ps);
        Assign(EnemyMeleeSO, ps);

        AssetDatabase.SaveAssets();
        Debug.Log("Slice7DamageTypeSetup: VFX wired on melee damage types.");
    }

    private static void Assign(string path, ParticleSystem ps)
    {
        var so = AssetDatabase.LoadAssetAtPath<DamageType>(path);
        if (so == null) { Debug.LogWarning($"DamageType not found at {path} — skipped."); return; }
        var serialized = new SerializedObject(so);
        serialized.FindProperty("VFX").objectReferenceValue = ps;
        serialized.ApplyModifiedProperties();
        EditorUtility.SetDirty(so);
    }
}
