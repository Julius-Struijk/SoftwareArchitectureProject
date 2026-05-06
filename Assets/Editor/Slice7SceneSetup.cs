using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using CMGTSA.Audio;
using CMGTSA.Feel;

/// <summary>
/// Idempotent slice-7 scene wiring. Adds FeedbackSystems + Audio GameObjects with all
/// Tier-1 controllers and their serialized references. Re-run safely; no duplicates.
/// Also wires PlayerHurtFlash, LevelUpVFXController, and SkillCastVFXController onto Player.prefab.
/// </summary>
public static class Slice7SceneSetup
{
    private const string CatalogPath        = "Assets/Scriptable Objects/Audio/AudioCatalog.asset";
    private const string DamageNumberPrefab = "Assets/Prefabs/Feel/DamageNumber.prefab";
    private const string EnemyDeathPrefab   = "Assets/Prefabs/Feel/EnemyDeathParticles.prefab";
    private const string DungeonMusicPath   = "Assets/Audio/Music/dungeon_loop.ogg";
    private const string BossMusicPath      = "Assets/Audio/Music/boss_loop.ogg";
    private const string PlayerPrefabPath   = "Assets/Prefabs/Player.prefab";
    private const string LevelUpPrefabPath  = "Assets/Prefabs/Feel/LevelUpParticles.prefab";

    [MenuItem("CMGTSA/Slice 7/Wire Scene")]
    public static void Execute()
    {
        // ── FeedbackSystems ──────────────────────────────────────────────────────
        var feedback = FindOrCreate("FeedbackSystems");
        EnsureComponent<HitStopController>(feedback);
        EnsureComponent<HitVFXSpawner>(feedback);
        var deathSpawner  = EnsureComponent<DeathVFXSpawner>(feedback);
        var numberSpawner = EnsureComponent<DamageNumberSpawner>(feedback);

        AssignSerialized(deathSpawner,  "enemyDeathPrefab",   AssetDatabase.LoadAssetAtPath<GameObject>(EnemyDeathPrefab));
        AssignSerialized(numberSpawner, "damageNumberPrefab", AssetDatabase.LoadAssetAtPath<GameObject>(DamageNumberPrefab));

        // ── Camera ──────────────────────────────────────────────────────────────
        var cam = Camera.main;
        if (cam != null)
            EnsureComponent<ScreenShakeController>(cam.gameObject);
        else
            Debug.LogWarning("Slice7SceneSetup: Camera.main not found; ScreenShakeController not wired.");

        // ── Audio ───────────────────────────────────────────────────────────────
        var audio = FindOrCreate("Audio");
        EnsureComponent<AudioSource>(audio);
        var sfx   = EnsureComponent<SFXController>(audio);
        var music = EnsureComponent<MusicController>(audio);

        var catalog  = AssetDatabase.LoadAssetAtPath<AudioCatalog>(CatalogPath);
        if (catalog != null) AssignSerialized(sfx, "catalog", catalog);
        else Debug.LogWarning("Slice7SceneSetup: AudioCatalog missing — run Slice7AudioSetup first.");

        var bgClip   = AssetDatabase.LoadAssetAtPath<AudioClip>(DungeonMusicPath);
        var bossClip = AssetDatabase.LoadAssetAtPath<AudioClip>(BossMusicPath);
        if (bgClip   != null) AssignSerialized(music, "bgClip",   bgClip);
        if (bossClip != null) AssignSerialized(music, "bossClip", bossClip);

        // ── Player Prefab feel components ────────────────────────────────────────
        WirePlayerPrefab();

        // ── Save ────────────────────────────────────────────────────────────────
        var scene = SceneManager.GetActiveScene();
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);
        UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
        AssetDatabase.SaveAssets();
        Debug.Log("Slice7SceneSetup: FeedbackSystems + Audio wired in active scene.");
    }

    private static void WirePlayerPrefab()
    {
        var playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath);
        if (playerPrefab == null) { Debug.LogError($"Missing {PlayerPrefabPath}"); return; }

        var instance = (GameObject)PrefabUtility.InstantiatePrefab(playerPrefab);
        try
        {
            EnsureComponent<PlayerHurtFlash>(instance);
            var lvl  = EnsureComponent<LevelUpVFXController>(instance);
            EnsureComponent<SkillCastVFXController>(instance);

            var lvlPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(LevelUpPrefabPath);
            if (lvlPrefab != null) AssignSerialized(lvl, "levelUpPrefab", lvlPrefab);

            PrefabUtility.SaveAsPrefabAsset(instance, PlayerPrefabPath);
        }
        finally
        {
            Object.DestroyImmediate(instance);
        }
    }

    // ── Helpers ─────────────────────────────────────────────────────────────────

    private static GameObject FindOrCreate(string name)
    {
        var existing = GameObject.Find(name);
        return existing != null ? existing : new GameObject(name);
    }

    private static T EnsureComponent<T>(GameObject go) where T : Component
    {
        var c = go.GetComponent<T>();
        if (c == null) c = go.AddComponent<T>();
        return c;
    }

    private static void AssignSerialized(Object obj, string fieldName, Object value)
    {
        if (obj == null || value == null) return;
        var so   = new SerializedObject(obj);
        var prop = so.FindProperty(fieldName);
        if (prop == null) { Debug.LogWarning($"Slice7SceneSetup: field '{fieldName}' not found on {obj}"); return; }
        prop.objectReferenceValue = value;
        so.ApplyModifiedProperties();
    }
}
