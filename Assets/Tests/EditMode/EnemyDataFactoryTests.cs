using NUnit.Framework;
using UnityEngine;
using CMGTSA.Enemies;

namespace CMGTSA.Tests.EditMode
{
    public class EnemyDataFactoryTests
    {
        [Test]
        public void CreateEnemy_WithPrefabAssigned_InstantiatesAtPosition()
        {
            var data = ScriptableObject.CreateInstance<EnemyData>();
            var prefab = new GameObject("EnemyPrefab");
            var field = typeof(EnemyData).GetField("prefab",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            Assert.NotNull(field, "EnemyData must have a private 'prefab' field");
            field.SetValue(data, prefab);

            GameObject spawned = data.CreateEnemy(new Vector3(2f, 3f, 0f));

            Assert.IsNotNull(spawned);
            Assert.AreEqual(new Vector3(2f, 3f, 0f), spawned.transform.position);

            Object.DestroyImmediate(spawned);
            Object.DestroyImmediate(prefab);
            Object.DestroyImmediate(data);
        }

        [Test]
        public void CreateEnemy_WithNullPrefab_ReturnsNull()
        {
            var data = ScriptableObject.CreateInstance<EnemyData>();

            GameObject spawned = data.CreateEnemy(Vector3.zero);

            Assert.IsNull(spawned);
            Object.DestroyImmediate(data);
        }
    }
}
