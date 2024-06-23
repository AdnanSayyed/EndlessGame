using UnityEngine;
using System.Collections.Generic;

public abstract class SpawnerBase : MonoBehaviour, ISpawner
{
    [SerializeField]
    protected SpawnableBase[] allPrefabs;

    protected List<SpawnableBase> spawnedObjects = new List<SpawnableBase>();

    public abstract void Spawn(GameObject platform, ObjectPooler objectPooler, ref float lastSpawnX);

    public virtual void ReturnToPool(ObjectPooler objectPooler)
    {
        foreach (SpawnableBase spawnedObject in spawnedObjects)
        {
            objectPooler.ReturnToPool(spawnedObject.SpawnableTag, spawnedObject.gameObject);
        }
        spawnedObjects.Clear();
    }

    public virtual void CheckAndReturnToPool(ObjectPooler objectPooler, Transform playerTransform, float offset = 15f)
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
}
