using UnityEditor;
using UnityEngine;

public static class InventoryPackageExporter
{
    private const string OutputPath = "Assets/Inventory.unitypackage";

    private static readonly string[] ExportPaths =
    {
        "Assets/Scripts/Inventory",
        "Assets/Scripts/UI/InventoryPanel.cs",
        "Assets/Scripts/UI/InventoryPresenter.cs",
        "Assets/Scripts/UI/InventorySlotView.cs",
        "Assets/Scripts/Player/PlayerInventoryContext.cs",
        "Assets/Prefabs/ItemPickup.prefab",
        "Assets/Prefabs/InventorySlotView.prefab",
        "Assets/Prefabs/InventoryPanel.prefab",
        "Assets/Scriptable Objects/Items",
    };

    [MenuItem("CMGTSA/Export Inventory Package")]
    public static void Export()
    {
        AssetDatabase.ExportPackage(
            ExportPaths,
            OutputPath,
            ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies);

        Debug.Log($"Inventory package exported to {OutputPath}");
        AssetDatabase.Refresh();
    }
}
