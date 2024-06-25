using EndlessGame.ObjectPool;
using EndlessGame.Spawnable;
using System.Collections.Generic;
using UnityEngine;

namespace EndlessGame.Spawner
{
    public abstract class SpawnerBase : MonoBehaviour, ISpawnerBase
    {
        [SerializeField]
        protected SpawnableBase[] allPrefabs;

        protected List<SpawnableBase> spawnedObjects = new List<SpawnableBase>();

        public abstract void Spawn(GameObject platform, IObjectPooler objectPooler, ref float lastSpawnX);

        public virtual void ReturnToPool(IObjectPooler objectPooler)
        {
            foreach (SpawnableBase spawnedObject in spawnedObjects)
            {
                objectPooler.ReturnToPool(spawnedObject.SpawnableTag, spawnedObject.gameObject);
            }
            spawnedObjects.Clear();
        }

        public virtual void CheckAndReturnToPool(IObjectPooler objectPooler, Transform playerTransform, float offset = 15f)
        {
            for (int i = spawnedObjects.Count - 1; i >= 0; i--)
            {
                if (spawnedObjects[i].transform.position.x < playerTransform.position.x - offset)
                {
                    objectPooler.ReturnToPool(spawnedObjects[i].SpawnableTag, spawnedObjects[i].gameObject);
                    spawnedObjects.RemoveAt(i);
                }
            }
        }

        public virtual void ResetService(IObjectPooler objectPooler)
        {
            ReturnToPool(objectPooler);
        }

    }
}
