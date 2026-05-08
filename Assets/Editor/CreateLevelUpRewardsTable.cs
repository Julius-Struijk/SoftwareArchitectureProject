using UnityEngine;
using UnityEditor;
using CMGTSA.Player;

public class CreateLevelUpRewardsTable
{
    public static void Execute()
    {
        // Ensure directory exists
        string dir = "Assets/Scriptable Objects/Player";
        if (!AssetDatabase.IsValidFolder("Assets/Scriptable Objects"))
            AssetDatabase.CreateFolder("Assets", "Scriptable Objects");
        if (!AssetDatabase.IsValidFolder(dir))
            AssetDatabase.CreateFolder("Assets/Scriptable Objects", "Player");

        string assetPath = dir + "/LevelUpRewardsTable.asset";

        // Don't overwrite if it exists
        var existing = AssetDatabase.LoadAssetAtPath<LevelUpRewardsTable>(assetPath);
        if (existing != null)
        {
            Debug.Log("LevelUpRewardsTable.asset already exists at " + assetPath);
            return;
        }

        var table = ScriptableObject.CreateInstance<LevelUpRewardsTable>();
        table.rewards = new LevelUpRewardsTable.LevelReward[]
        {
            new LevelUpRewardsTable.LevelReward { level = 2, hpDelta = 2, damageDelta = 1, healToFull = true },
            new LevelUpRewardsTable.LevelReward { level = 3, hpDelta = 2, damageDelta = 1, healToFull = true },
            new LevelUpRewardsTable.LevelReward { level = 4, hpDelta = 3, damageDelta = 2, healToFull = true },
            new LevelUpRewardsTable.LevelReward { level = 5, hpDelta = 4, damageDelta = 2, healToFull = true },
        };
        table.fallback = new LevelUpRewardsTable.LevelReward
        {
            level = 0, hpDelta = 2, damageDelta = 1, healToFull = true
        };

        AssetDatabase.CreateAsset(table, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Created LevelUpRewardsTable.asset at " + assetPath);
    }
}
