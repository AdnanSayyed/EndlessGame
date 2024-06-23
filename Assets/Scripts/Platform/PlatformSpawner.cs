using System.Collections.Generic;
using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    public Platform[] basePlatformPrefabs;
    public float basePlatformSpawnInterval = 1f;

    private Transform playerTransform;
    private Vector3 lastBasePlatformPosition;
    private float lastPlatformWidth;
    private ObjectPooler objectPooler;

    private Vector3 initialSpawnPosition = new Vector3(-5, 0, 0);
    private List<Transform> activePlatforms = new List<Transform>();  // List to keep track of active platforms

    // Variables to track last spawn positions
    private float lastSpawnX;

    private ObstacleSpawner obstacleSpawner;
    private CollectableSpawner collectableSpawner;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag(Constants.PlayerTag).transform;
        lastBasePlatformPosition = Vector3.zero;
        lastPlatformWidth = 0f;

        objectPooler = ObjectPooler.Instance;

        obstacleSpawner = GetComponent<ObstacleSpawner>();
        collectableSpawner = GetComponent<CollectableSpawner>();

        // Initial platform spawn to start the game
        SpawnInitialBasePlatforms();
    }

    void Update()
    {
        // Check if a new base platform needs to be spawned
        if (playerTransform.position.x + 10 > lastBasePlatformPosition.x)
        {
            SpawnBasePlatform();
        }

        // Return platforms to the pool if they are off-screen
        for (int i = activePlatforms.Count - 1; i >= 0; i--)
        {
            GameObject platform = activePlatforms[i].gameObject;
            float platformWidth = platform.GetComponent<Platform>().Length;

            if (platform.transform.position.x + platformWidth / 2 < playerTransform.position.x - 15)
            {
                Platform platformComponent = platform.GetComponent<Platform>();
                objectPooler.ReturnToPool(platformComponent.SpawnableTag, platform);
                activePlatforms.RemoveAt(i);
            }
        }

        // Check and return obstacles and collectables to the pool if they are off-screen
        obstacleSpawner.CheckAndReturnToPool(objectPooler, playerTransform);
        collectableSpawner.CheckAndReturnToPool(objectPooler, playerTransform);
    }

    void SpawnInitialBasePlatforms()
    {
        for (int i = 0; i < 10; i++)
        {
            SpawnBasePlatform();
        }
    }

    void SpawnBasePlatform()
    {
        Platform platformToSpawn = basePlatformPrefabs[Random.Range(0, basePlatformPrefabs.Length)];
        GameObject newPlatform = objectPooler.SpawnFromPool(platformToSpawn.SpawnableTag, initialSpawnPosition, Quaternion.identity);

        Vector3 spawnPosition;

        if (lastBasePlatformPosition == Vector3.zero)
        {
            spawnPosition = initialSpawnPosition;
        }
        else
        {
            spawnPosition = lastBasePlatformPosition + Vector3.right * (lastPlatformWidth / 2f + basePlatformSpawnInterval);
        }

        newPlatform.transform.position = spawnPosition;
        float platformWidth = newPlatform.GetComponent<Platform>().Length;

        lastBasePlatformPosition = spawnPosition + Vector3.right * (platformWidth / 2f);

        lastPlatformWidth = platformWidth;

        activePlatforms.Add(newPlatform.transform);

        obstacleSpawner.Spawn(newPlatform, objectPooler, ref lastSpawnX);
        collectableSpawner.Spawn(newPlatform, objectPooler, ref lastSpawnX);
    }
}
