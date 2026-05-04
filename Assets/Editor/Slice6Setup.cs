using System.IO;
using UnityEditor;
using UnityEngine;
using CMGTSA.Battle;
using CMGTSA.Boss;

/// <summary>
/// Idempotent Editor setup for slice 6. Creates 3 pattern assets, 3 phase assets, and 1
/// BossData asset under Assets/Scriptable Objects/Boss/. Run via Coplay execute_script —
/// call <see cref="Execute"/> after a clean compile. Run Slice6PrefabSetup first so the
/// projectile prefab is available to wire into the spray pattern.
/// </summary>
public class Slice6Setup
{
    private const string BossDir      = "Assets/Scriptable Objects/Boss";
    private const string PatternsDir  = "Assets/Scriptable Objects/Boss/Patterns";
    private const string PhasesDir    = "Assets/Scriptable Objects/Boss/Phases";
    private const string ProjectilePrefabPath = "Assets/Prefabs/Boss/BossProjectile.prefab";
    private const string AddPrefabPath        = "Assets/Prefabs/Ghost.prefab";

    public static void Execute()
    {
        EnsureDir(BossDir);
        EnsureDir(PatternsDir);
        EnsureDir(PhasesDir);

        GameObject projectilePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(ProjectilePrefabPath);
        GameObject addPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(AddPrefabPath);
        if (projectilePrefab == null) Debug.LogWarning($"Slice6Setup: missing {ProjectilePrefabPath}. Run Slice6PrefabSetup first.");
        if (addPrefab == null) Debug.LogWarning($"Slice6Setup: missing {AddPrefabPath}. Wire the add prefab manually.");

        var melee = CreateOrLoad<MeleeSlamPattern>("Pattern_MeleeSlam", PatternsDir, p =>
        {
            p.cooldownSeconds = 2f;
            p.range = 1.5f;
            p.windupSeconds = 0.6f;
            p.damage = new DamageData { damage = 6 };
        });

        var spray = CreateOrLoad<ProjectileSprayPattern>("Pattern_ProjectileSpray", PatternsDir, p =>
        {
            p.cooldownSeconds = 3f;
            p.range = 6f;
            p.projectilePrefab = projectilePrefab;
            p.projectileCount = 5;
            p.spreadDegrees = 40f;
            p.projectileSpeed = 5f;
            p.intervalSeconds = 0.15f;
            p.damage = new DamageData { damage = 2 };
        });

        var summon = CreateOrLoad<SummonAddsPattern>("Pattern_SummonAdds", PatternsDir, p =>
        {
            p.cooldownSeconds = 8f;
            p.range = 0f;
            p.addPrefab = addPrefab;
            p.addCount = 3;
            p.spawnRadius = 2.5f;
        });

        var phase1 = CreateOrLoad<BossPhase>("Phase1_FullHP", PhasesDir, p =>
        {
            p.displayName = "Phase 1 — Aggressive Melee";
            p.hpThresholdEnter = 1.0f;
            p.castIntervalSeconds = 2f;
            p.patterns = new SOBossAttackPattern[] { melee, spray };
        });

        var phase2 = CreateOrLoad<BossPhase>("Phase2_HalfHP", PhasesDir, p =>
        {
            p.displayName = "Phase 2 — Ranged Pressure";
            p.hpThresholdEnter = 0.66f;
            p.castIntervalSeconds = 1.6f;
            p.patterns = new SOBossAttackPattern[] { spray, melee, spray };
        });

        var phase3 = CreateOrLoad<BossPhase>("Phase3_LowHP", PhasesDir, p =>
        {
            p.displayName = "Phase 3 — Desperate Summons";
            p.hpThresholdEnter = 0.33f;
            p.castIntervalSeconds = 1.2f;
            p.patterns = new SOBossAttackPattern[] { summon, spray, melee };
        });

        CreateOrLoad<BossData>("Boss_LichKing", BossDir, b =>
        {
            b.displayName = "Lich King";
            b.maxHP = 60;
            b.moveSpeed = 1.8f;
            b.engagementRange = 4f;
            b.phases = new BossPhase[] { phase1, phase2, phase3 };
            b.xpReward = 100;
            b.moneyReward = 50;
        });

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Slice6Setup: 3 patterns + 3 phases + Boss_LichKing created/updated.");
    }

    private static T CreateOrLoad<T>(string fileName, string dir, System.Action<T> configure) where T : ScriptableObject
    {
        string path = $"{dir}/{fileName}.asset";
        T asset = AssetDatabase.LoadAssetAtPath<T>(path);
        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
        }
        configure(asset);
        EditorUtility.SetDirty(asset);
        return asset;
    }

    private static void EnsureDir(string path)
    {
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
    }
}
