using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Idempotent slice-7 particle-prefab creator. Run via Coplay MCP execute_script.
/// Each call recreates prefabs from defaults — safe to re-run.
/// </summary>
public static class Slice7PrefabSetup
{
    private const string PrefabFolder = "Assets/Prefabs/Feel";

    [MenuItem("CMGTSA/Slice 7/Create Particle Prefabs")]
    public static void Execute()
    {
        EnsureFolder(PrefabFolder);

        CreateParticlePrefab("HitParticles",
            startColor: new Color(1f, 0.95f, 0.6f),
            startLifetime: 0.4f, startSpeed: 3f, emissionBurst: 8, duration: 0.4f,
            startSize: 0.15f);

        CreateParticlePrefab("EnemyDeathParticles",
            startColor: new Color(0.4f, 0.4f, 0.4f),
            startLifetime: 0.8f, startSpeed: 2.5f, emissionBurst: 24, duration: 0.6f,
            startSize: 0.2f);

        CreateParticlePrefab("LevelUpParticles",
            startColor: new Color(1f, 0.85f, 0.3f),
            startLifetime: 1.2f, startSpeed: 2.5f, emissionBurst: 40, duration: 1.0f,
            startSize: 0.18f, upwardBias: 1.5f);

        CreateParticlePrefab("SkillCastParticles",
            startColor: new Color(0.6f, 0.3f, 0.95f),
            startLifetime: 0.6f, startSpeed: 4f, emissionBurst: 20, duration: 0.5f,
            startSize: 0.18f);

        CreateDamageNumberPrefab();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Slice7PrefabSetup: particle prefabs created in " + PrefabFolder);
    }

    private static void CreateParticlePrefab(string name, Color startColor,
        float startLifetime, float startSpeed, int emissionBurst, float duration,
        float startSize, float upwardBias = 0f)
    {
        var go = new GameObject(name);
        var ps = go.AddComponent<ParticleSystem>();

        var main = ps.main;
        main.duration       = duration;
        main.loop           = false;
        main.startLifetime  = startLifetime;
        main.startSpeed     = startSpeed;
        main.startSize      = startSize;
        main.startColor     = startColor;
        main.gravityModifier = -upwardBias;
        main.stopAction     = ParticleSystemStopAction.Destroy;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = ps.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, (short)emissionBurst) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.15f;

        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.sharedMaterial = AssetDatabase.GetBuiltinExtraResource<Material>(
            "Default-ParticleSystem.mat");

        string path = $"{PrefabFolder}/{name}.prefab";
        PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
    }

    private static void CreateDamageNumberPrefab()
    {
        var go = new GameObject("DamageNumber");
        var text = go.AddComponent<TMPro.TextMeshPro>();
        text.text       = "0";
        text.fontSize   = 6f;
        text.color      = Color.white;
        text.alignment  = TMPro.TextAlignmentOptions.Center;
        text.fontStyle  = TMPro.FontStyles.Bold;
        text.outlineWidth = 0.2f;
        text.outlineColor = Color.black;
        var rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(2f, 1f);

        go.AddComponent<CMGTSA.Feel.DamageNumber>();

        string path = $"{PrefabFolder}/DamageNumber.prefab";
        PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
    }

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path)) return;
        var parent = Path.GetDirectoryName(path).Replace('\\', '/');
        var leaf   = Path.GetFileName(path);
        if (!AssetDatabase.IsValidFolder(parent)) EnsureFolder(parent);
        AssetDatabase.CreateFolder(parent, leaf);
    }
}
