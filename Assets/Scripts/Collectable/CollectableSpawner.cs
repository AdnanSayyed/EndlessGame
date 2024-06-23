using UnityEngine;

public class CollectableSpawner : SpawnerBase
{
    [SerializeField] float collectableSpawnChance = 0.5f; 
    [SerializeField] float minCollectableDistance = 3f; 

    public override void Spawn(GameObject platform, ObjectPooler objectPooler, ref float lastSpawnX)
    {
        float platformX = platform.transform.position.x;
        float platformWidth = platform.GetComponent<Platform>().Length;

        if (Random.value < collectableSpawnChance && platformX - lastSpawnX >= minCollectableDistance)
        {
            Collectable collectableToSpawn = (Collectable)allPrefabs[Random.Range(0, allPrefabs.Length)];
            float yOffset = collectableToSpawn.SpawnYOffset;

            Vector3 collectablePosition = new Vector3(
                platform.transform.position.x,
                platform.transform.position.y + yOffset,
                platform.transform.position.z
            );
            GameObject collectable = objectPooler.SpawnFromPool(collectableToSpawn.SpawnableTag, collectablePosition, Quaternion.identity);

            spawnedObjects.Add(collectable.GetComponent<SpawnableBase>());

            lastSpawnX = platformX;
        }
    }

    public override void ReturnToPool(ObjectPooler objectPooler)
    {
        base.ReturnToPool(objectPooler);
    }
}
