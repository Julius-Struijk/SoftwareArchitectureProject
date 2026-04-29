using System.IO;
using UnityEditor;
using UnityEngine;
using CMGTSA.Battle;
using CMGTSA.Skills;

/// <summary>
/// One-shot Editor setup for slice 5. Creates three SOSkillEffect assets (SpeedBuff,
/// AOESlam, Blink) and three SkillData assets that reference them. Run via Coplay
/// execute_script — call <see cref="Execute"/> after a clean compile.
/// </summary>
public class Slice5Setup
{
    private const string SkillsDir   = "Assets/Scriptable Objects/Skills";
    private const string EffectsDir  = "Assets/Scriptable Objects/Skills/Effects";

    public static void Execute()
    {
        EnsureDir(SkillsDir);
        EnsureDir(EffectsDir);

        var speed = CreateOrLoadEffect<SpeedBuffEffect>("Effect_SpeedBuff", e =>
        {
            e.multiplier = 1.25f;
        });

        var slam = CreateOrLoadEffect<AOESlamEffect>("Effect_AOESlam", e =>
        {
            e.radius = 2.5f;
            e.damage = new DamageData { damage = 3 };
        });

        var blink = CreateOrLoadEffect<BlinkEffect>("Effect_Blink", e =>
        {
            e.distance = 4f;
        });

        CreateOrLoadSkill("Skill_SpeedBuff", s =>
        {
            s.displayName = "Fleet of Foot";
            s.description = "+25% movement speed.";
            s.requiredLevel = 1;
            s.cooldownSeconds = 0f;
            s.effect = speed;
        });

        CreateOrLoadSkill("Skill_AOESlam", s =>
        {
            s.displayName = "Slam";
            s.description = "Damage every enemy in a small radius.";
            s.requiredLevel = 2;
            s.cooldownSeconds = 6f;
            s.effect = slam;
        });

        CreateOrLoadSkill("Skill_Blink", s =>
        {
            s.displayName = "Blink";
            s.description = "Dash forward instantly.";
            s.requiredLevel = 3;
            s.cooldownSeconds = 4f;
            s.effect = blink;
        });

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Slice5Setup: 3 effect SOs + 3 skill SOs created/updated.");
    }

    private static T CreateOrLoadEffect<T>(string fileName, System.Action<T> configure) where T : SOSkillEffect
    {
        string path = $"{EffectsDir}/{fileName}.asset";
        var asset = AssetDatabase.LoadAssetAtPath<T>(path);
        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
        }
        configure(asset);
        EditorUtility.SetDirty(asset);
        return asset;
    }

    private static SkillData CreateOrLoadSkill(string fileName, System.Action<SkillData> configure)
    {
        string path = $"{SkillsDir}/{fileName}.asset";
        var asset = AssetDatabase.LoadAssetAtPath<SkillData>(path);
        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<SkillData>();
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
