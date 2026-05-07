using UnityEditor;
using UnityEngine;
using CMGTSA.Game;

namespace CMGTSA.Editor
{
    /// <summary>
    /// Idempotent setup for slice 8: ensures a <c>DebugInput</c> child exists under
    /// the <c>GameSystems</c> root with a <see cref="DebugInputController"/>
    /// component attached. Safe to re-run.
    /// </summary>
    public static class Slice8DebugSetup
    {
        private const string SystemsRootName = "GameSystems";
        private const string DebugChildName  = "DebugInput";

        [MenuItem("CMGTSA/Slice 8/Add Debug Input")]
        public static void AddDebugInput()
        {
            var rootGo = GameObject.Find(SystemsRootName);
            if (rootGo == null)
            {
                Debug.LogError($"[Slice8DebugSetup] No '{SystemsRootName}' GameObject in active scene.");
                return;
            }

            var existing = rootGo.transform.Find(DebugChildName);
            if (existing != null && existing.GetComponent<DebugInputController>() != null)
            {
                Debug.Log("[Slice8DebugSetup] DebugInput already present — nothing to do.");
                return;
            }

            var child = existing != null
                ? existing.gameObject
                : new GameObject(DebugChildName);
            child.transform.SetParent(rootGo.transform, worldPositionStays: false);
            if (child.GetComponent<DebugInputController>() == null)
            {
                child.AddComponent<DebugInputController>();
            }

            EditorUtility.SetDirty(child);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(child.scene);
            Debug.Log("[Slice8DebugSetup] Added DebugInput → DebugInputController.");
        }
    }
}
