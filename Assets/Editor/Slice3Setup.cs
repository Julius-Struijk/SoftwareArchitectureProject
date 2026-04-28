using System.IO;
using UnityEditor;
using UnityEngine;
using CMGTSA.Inventory;
using CMGTSA.Enemies;

/// <summary>
/// One-shot Editor setup for slice 3. Creates the three ItemCategory SOs, the three
/// ItemData SOs (HPPotion, IronCharm, OldKey), and writes loot tables onto the
/// existing Ghost.asset and Golem.asset. Run via Coplay execute_script — call
/// <see cref="Execute"/> after a clean compile.
/// </summary>
public class Slice3Setup
{
    private const string ItemsDir      = "Assets/Scriptable Objects/Items";
    private const string CategoriesDir = "Assets/Scriptable Objects/Items/Categories";

    public static void Execute()
    {
        EnsureDir(ItemsDir);
        EnsureDir(CategoriesDir);
        var consumable = CreateOrLoadCategory("ItemCategory_Consumable",   "Consumable",    new Color(0.95f, 0.4f, 0.4f));
        var passive    = CreateOrLoadCategory("ItemCategory_PassiveEquip", "Passive Equip", new Color(0.5f, 0.7f, 1f));
        var quest      = CreateOrLoadCategory("ItemCategory_QuestItem",    "Quest Item",    new Color(1f, 0.9f, 0.3f));

        var hpPotion  = CreateOrLoadItem("HPPotion", item =>
        {
            item.displayName = "HP Potion";
            item.category = consumable;
            item.isStackable = true;
            item.consumableHealAmount = 4;
            item.attackBonus = 0;
            item.description = "Restores 4 HP. Stacks.";
        });
        var ironCharm = CreateOrLoadItem("IronCharm", item =>
        {
            item.displayName = "Iron Charm";
            item.category = passive;
            item.isStackable = false;
            item.consumableHealAmount = 0;
            item.attackBonus = 2;
            item.description = "+2 attack while equipped (slice 5 wires real buff).";
        });
        var oldKey    = CreateOrLoadItem("OldKey", item =>
        {
            item.displayName = "Old Key";
            item.category = quest;
            item.isStackable = false;
            item.consumableHealAmount = 0;
            item.attackBonus = 0;
            item.description = "Unlocks something. Reserved for slice 4 quest.";
        });

        WriteLootTable("Assets/Scriptable Objects/Ghost.asset", new[]
        {
            new LootEntry { item = hpPotion, chance = 0.5f },
            new LootEntry { item = oldKey,   chance = 0.25f },
        });
        WriteLootTable("Assets/Scriptable Objects/Golem.asset", new[]
        {
            new LootEntry { item = hpPotion,  chance = 0.5f },
            new LootEntry { item = ironCharm, chance = 0.25f },
        });

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Slice3Setup: 3 categories + 3 items + Ghost/Golem loot tables done. " +
                  "ICONS NOT SET — drag sprites from Assets/Sprites onto " +
                  "the three ItemData assets manually.");
    }

    private static ItemCategory CreateOrLoadCategory(string fileName, string displayName, Color tint)
    {
        string path = $"{CategoriesDir}/{fileName}.asset";
        var asset = AssetDatabase.LoadAssetAtPath<ItemCategory>(path);
        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<ItemCategory>();
            AssetDatabase.CreateAsset(asset, path);
        }
        asset.displayName = displayName;
        asset.tint = tint;
        EditorUtility.SetDirty(asset);
        return asset;
    }

    private static ItemData CreateOrLoadItem(string fileName, System.Action<ItemData> configure)
    {
        string path = $"{ItemsDir}/{fileName}.asset";
        var asset = AssetDatabase.LoadAssetAtPath<ItemData>(path);
        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<ItemData>();
            AssetDatabase.CreateAsset(asset, path);
        }
        configure(asset);
        EditorUtility.SetDirty(asset);
        return asset;
    }

    private static void WriteLootTable(string enemyAssetPath, LootEntry[] entries)
    {
        var data = AssetDatabase.LoadAssetAtPath<EnemyData>(enemyAssetPath);
        if (data == null) { Debug.LogError($"{enemyAssetPath} not found."); return; }
        data.lootTable = entries;
        EditorUtility.SetDirty(data);
    }

    private static void EnsureDir(string path)
    {
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
    }
}
