using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using CMGTSA.Boss;

namespace CMGTSA.Tests.PlayMode
{
    /// <summary>
    /// PlayMode regression: SummonAddsRuntime through the real
    /// BossPatternContext + Unity NavMesh must never spawn an add inside
    /// a wall collider. EditMode covers the retry/skip logic via a fake;
    /// this test covers the Unity-side NavMesh.SamplePosition call.
    /// </summary>
    public class SummonAddsNavMeshTests
    {
        [UnityTest]
        public IEnumerator Tick_DoesNotSpawnInsideWallCollider()
        {
            yield return SceneManager.LoadSceneAsync(
                "Assets/Scenes/Tests/SummonAddsNavMeshScene.unity",
                LoadSceneMode.Single);

            GameObject wall = GameObject.Find("Wall");
            Assert.IsNotNull(wall, "Test scene missing a 'Wall' GameObject.");
            Collider2D wallCollider = wall.GetComponent<Collider2D>();
            Assert.IsNotNull(wallCollider, "'Wall' must have a Collider2D in the test scene.");

            GameObject boss = new GameObject("FakeBoss");
            boss.transform.position = new Vector3(2.5f, 0f, 0f);
            GameObject player = new GameObject("FakePlayer");
            var ctx = new BossPatternContext(boss.transform, player.transform);

            GameObject addPrefab = new GameObject("AddDummy");

            var pattern = ScriptableObject.CreateInstance<SummonAddsPattern>();
            pattern.addPrefab = addPrefab;
            pattern.addCount = 8;
            pattern.spawnRadius = 3f;
            pattern.maxAttempts = 6;
            pattern.sampleDistance = 1f;

            var rt = pattern.CreateRuntime();
            rt.OnBegin(ctx);
            rt.Tick(0f, ctx);

            int spawned = 0;
            foreach (var go in Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
            {
                if (go.name == "AddDummy(Clone)")
                {
                    spawned++;
                    Vector2 p = go.transform.position;
                    Assert.IsFalse(wallCollider.OverlapPoint(p),
                        $"Add at {p} was spawned inside the wall collider.");
                }
            }

            Assert.Greater(spawned, 0, "Expected at least one add to spawn (some sides of the ring are clear).");

            Object.DestroyImmediate(boss);
            Object.DestroyImmediate(player);
            Object.DestroyImmediate(addPrefab);
            Object.DestroyImmediate(pattern);
        }
    }
}
