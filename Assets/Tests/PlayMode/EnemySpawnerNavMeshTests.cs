using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.AI;
using CMGTSA.Spawner;

namespace CMGTSA.Tests.PlayMode
{
    /// <summary>
    /// PlayMode regression: EnemySpawner against a real baked NavMesh must spawn enemies
    /// inside the room's BoxCollider2D bounds and on the NavMesh (validates that the
    /// runtime UnityNavMeshSampler + factory delegate chain works end-to-end).
    /// </summary>
    public class EnemySpawnerNavMeshTests
    {
        [UnityTest]
        public IEnumerator Spawner_PopulatesRoom_OverFiveSeconds()
        {
            yield return SceneManager.LoadSceneAsync(
                "Assets/Scenes/Tests/EnemySpawnerNavMeshScene.unity",
                LoadSceneMode.Single);

            yield return null;
            yield return new WaitForSeconds(5f);

            var room = Object.FindAnyObjectByType<Room>();
            Assert.IsNotNull(room, "Test scene must contain a Room.");

            var roomCollider = room.GetComponent<BoxCollider2D>();
            Assert.IsNotNull(roomCollider, "Test room must have a BoxCollider2D.");

            int sampled = 0;
            foreach (var tag in Object.FindObjectsByType<EnemyRoomTag>(FindObjectsSortMode.None))
            {
                if (tag.Room != room) continue;
                sampled++;
                Vector2 p = tag.transform.position;
                Assert.IsTrue(roomCollider.OverlapPoint(p),
                    $"Spawned enemy at {p} is outside the room's BoxCollider2D bounds.");
                Assert.IsTrue(NavMesh.SamplePosition(tag.transform.position, out _, 0.5f, NavMesh.AllAreas),
                    $"Spawned enemy at {p} is not on the NavMesh.");
            }

            Assert.Greater(sampled, 0, "Expected at least one spawned enemy within 5 seconds.");
            Assert.LessOrEqual(sampled, room.SimultaneousCap, "Spawner must not exceed cap.");
        }
    }
}
