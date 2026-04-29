using System.IO;
using UnityEditor;
using UnityEngine;
using CMGTSA.Enemies;
using CMGTSA.Inventory;
using CMGTSA.Quests;

/// <summary>
/// One-shot Editor setup for slice 4. Creates two SOQuestGoal assets (one KillEnemyGoal,
/// one FetchItemGoal) and two QuestData assets that reference them. Run via Coplay
/// execute_script — call <see cref="Execute"/> after a clean compile.
/// </summary>
public class Slice4Setup
{
    private const string QuestsDir = "Assets/Scriptable Objects/Quests";
    private const string GoalsDir  = "Assets/Scriptable Objects/Quests/Goals";

    private const string GhostAssetPath  = "Assets/Scriptable Objects/Ghost.asset";
    private const string OldKeyAssetPath = "Assets/Scriptable Objects/Items/OldKey.asset";

    public static void Execute()
    {
        EnsureDir(QuestsDir);
        EnsureDir(GoalsDir);

        var ghost  = AssetDatabase.LoadAssetAtPath<EnemyData>(GhostAssetPath);
        var oldKey = AssetDatabase.LoadAssetAtPath<ItemData>(OldKeyAssetPath);
        if (ghost == null)  { Debug.LogError($"{GhostAssetPath} not found — slice 2/3 incomplete?"); return; }
        if (oldKey == null) { Debug.LogError($"{OldKeyAssetPath} not found — slice 3 incomplete?"); return; }

        var killGhosts = CreateOrLoadGoal<KillEnemyGoal>("Goal_KillTwoGhosts", g =>
        {
            g.targetEnemy = ghost;
            g.requiredCount = 2;
        });

        var fetchKey = CreateOrLoadGoal<FetchItemGoal>("Goal_FetchOldKey", g =>
        {
            g.targetItem = oldKey;
            g.requiredCount = 1;
        });

        CreateOrLoadQuest("Quest_CullTheGhosts", q =>
        {
            q.displayName = "Cull the Ghosts";
            q.description = "The crypt is overrun. Thin them out.";
            q.goals = new SOQuestGoal[] { killGhosts };
            q.xpReward = 5;
        });

        CreateOrLoadQuest("Quest_FindTheOldKey", q =>
        {
            q.displayName = "Find the Old Key";
            q.description = "It opens something. Probably.";
            q.goals = new SOQuestGoal[] { fetchKey };
            q.xpReward = 5;
        });

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Slice4Setup: 2 goals + 2 quests created/updated.");
    }

    private static T CreateOrLoadGoal<T>(string fileName, System.Action<T> configure) where T : SOQuestGoal
    {
        string path = $"{GoalsDir}/{fileName}.asset";
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

    private static QuestData CreateOrLoadQuest(string fileName, System.Action<QuestData> configure)
    {
        string path = $"{QuestsDir}/{fileName}.asset";
        var asset = AssetDatabase.LoadAssetAtPath<QuestData>(path);
        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<QuestData>();
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
