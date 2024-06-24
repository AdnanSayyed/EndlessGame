using EndlessGame.ObjectPool;
using EndlessGame.Player;
using EndlessGame.Service;
using EndlessGame.Spawner;
using System.Collections.Generic;
using UnityEngine;

namespace EndlessGame.Spawnable
{
    public class PlatformSpawner : SpawnerBase, IPlatformSpawner
    {
        public float basePlatformSpawnInterval = 1f;

        private Transform playerTransform;
        private Vector3 lastBasePlatformPosition;
        private float lastPlatformWidth;
        private IObjectPooler objectPooler;
        private Vector3 initialSpawnPosition = new Vector3(-5, 0, 0);
        private List<Transform> activePlatforms = new List<Transform>();

        public void Initialize()
        {
            playerTransform = ServiceLocator.GetService<IPlayerController>().GetTransform();
            lastBasePlatformPosition = Vector3.zero;
            lastPlatformWidth = 0f;

            objectPooler = ServiceLocator.GetService<IObjectPooler>();

            SpawnInitialBasePlatforms();
        }

        public void UpdateSpawner()
        {
            if (playerTransform.position.x + 10 > lastBasePlatformPosition.x)
            {
                SpawnPlatform();
            }

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
        }

        public override void Spawn(GameObject parent, IObjectPooler objectPooler, ref float lastSpawnX)
        {
            Platform platformToSpawn = allPrefabs[Random.Range(0, allPrefabs.Length)] as Platform;
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

            lastSpawnX = newPlatform.transform.position.x;
            spawnedObjects.Add(newPlatform.GetComponent<SpawnableBase>());
        }

        private void SpawnInitialBasePlatforms()
        {
            for (int i = 0; i < 10; i++)
            {
                SpawnPlatform();
            }
        }

        private void SpawnPlatform()
        {
            float temp = 0f;
            Spawn(gameObject, objectPooler, ref temp);
        }

        public List<Transform> GetActivePlatforms()
        {
            return activePlatforms;
        }
    }

}