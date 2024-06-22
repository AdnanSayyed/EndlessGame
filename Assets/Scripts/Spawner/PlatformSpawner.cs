using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    public Platform[] basePlatformPrefabs;
    public float basePlatformSpawnInterval = 1f;

    private Transform playerTransform;
    private Vector3 lastBasePlatformPosition;
    private float lastPlatformWidth;

    private Vector2 initialSpawnPosition = new Vector3(-5, 0, 0);

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag(Constants.PlayerTag).transform;
        lastBasePlatformPosition = Vector3.zero;
        lastPlatformWidth = 0f;

        SpawnInitialBasePlatforms();
    }

    void Update()
    {
        if (playerTransform.position.x + 10 > lastBasePlatformPosition.x)
        {
            SpawnBasePlatform();
        }

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
        GameObject prefabToSpawn = basePlatformPrefabs[Random.Range(0, basePlatformPrefabs.Length)].gameObject;

        Vector3 spawnPosition;

        if (lastBasePlatformPosition == Vector3.zero)
        {
            spawnPosition = initialSpawnPosition;
        }
        else
        {
            spawnPosition = lastBasePlatformPosition + Vector3.right * (lastPlatformWidth / 2f + basePlatformSpawnInterval);
        }

        GameObject platformInstance = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        float platformWidth = platformInstance.GetComponent<Platform>().Length;

        lastBasePlatformPosition = spawnPosition + Vector3.right * (platformWidth / 2f);

        lastPlatformWidth = platformWidth;
    }
}
