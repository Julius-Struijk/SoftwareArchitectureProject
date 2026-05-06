using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using CMGTSA.Audio;

/// <summary>
/// Idempotent slice-7 audio bootstrap: walks Assets/Audio/SFX + Assets/Audio/Music for
/// known filenames, builds an AudioCatalog asset keyed by <see cref="AudioSlot"/>, and
/// saves it under Assets/Scriptable Objects/Audio/AudioCatalog.asset.
/// </summary>
public static class Slice7AudioSetup
{
    private const string SFXFolder    = "Assets/Audio/SFX";
    private const string MusicFolder  = "Assets/Audio/Music";
    private const string CatalogPath  = "Assets/Scriptable Objects/Audio/AudioCatalog.asset";

    private static readonly (AudioSlot Slot, string Filename)[] SfxMap = new[]
    {
        (AudioSlot.AttackWhoosh, "attack_whoosh"),
        (AudioSlot.PlayerHit,    "hit_player"),
        (AudioSlot.EnemyHit,     "hit_enemy"),
        (AudioSlot.EnemyDeath,   "death_enemy"),
        (AudioSlot.PlayerDeath,  "death_player"),
        (AudioSlot.LevelUp,      "level_up"),
        (AudioSlot.SkillCast,    "skill_cast"),
        (AudioSlot.BossStinger,  "boss_stinger"),
        (AudioSlot.UIClick,      "ui_click"),
    };

    [MenuItem("CMGTSA/Slice 7/Build AudioCatalog")]
    public static void Execute()
    {
        EnsureFolder("Assets/Scriptable Objects/Audio");
        EnsureFolder(SFXFolder);
        EnsureFolder(MusicFolder);

        var catalog = AssetDatabase.LoadAssetAtPath<AudioCatalog>(CatalogPath);
        if (catalog == null)
        {
            catalog = ScriptableObject.CreateInstance<AudioCatalog>();
            AssetDatabase.CreateAsset(catalog, CatalogPath);
        }

        var entries = new List<AudioCatalog.Entry>();
        foreach (var (slot, filename) in SfxMap)
        {
            AudioClip clip = LoadClip(SFXFolder, filename);
            if (clip == null)
            {
                Debug.LogWarning($"Slice7AudioSetup: missing {filename} in {SFXFolder} — slot {slot} empty.");
                continue;
            }
            entries.Add(new AudioCatalog.Entry { slot = slot, clip = clip, volume = 1f });
        }

        var so = new SerializedObject(catalog);
        var arr = so.FindProperty("entries");
        arr.arraySize = entries.Count;
        for (int i = 0; i < entries.Count; i++)
        {
            var elem = arr.GetArrayElementAtIndex(i);
            elem.FindPropertyRelative("slot").enumValueIndex        = (int)entries[i].slot;
            elem.FindPropertyRelative("clip").objectReferenceValue  = entries[i].clip;
            elem.FindPropertyRelative("volume").floatValue          = entries[i].volume;
        }
        so.ApplyModifiedProperties();

        EditorUtility.SetDirty(catalog);
        AssetDatabase.SaveAssets();
        Debug.Log($"Slice7AudioSetup: wrote {entries.Count} entries to AudioCatalog.");
    }

    private static AudioClip LoadClip(string folder, string baseName)
    {
        foreach (var ext in new[] { ".wav", ".ogg", ".mp3" })
        {
            var path = $"{folder}/{baseName}{ext}";
            var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            if (clip != null) return clip;
        }
        return null;
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
