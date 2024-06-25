using EndlessGame.ObjectPool;
using EndlessGame.Player;
using EndlessGame.Service;
using EndlessGame.Spawner;
using System.Collections.Generic;
using Unity.VisualScripting;
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

            for (int i = spawnedObjects.Count - 1; i >= 0; i--)
            {
                GameObject platform = spawnedObjects[i].gameObject;
                float platformWidth = platform.GetComponent<Platform>().Length;

                if (platform.transform.position.x + platformWidth / 2 < playerTransform.position.x - 15)
                {
                    Platform platformComponent = platform.GetComponent<Platform>();
                    objectPooler.ReturnToPool(platformComponent.SpawnableTag, platform);
                    spawnedObjects.RemoveAt(i);
                    Debug.Log("REMOVING PLATFORM " + platformComponent.gameObject.name);
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


            lastSpawnX = newPlatform.transform.position.x;
            spawnedObjects.Add(newPlatform.GetComponent<SpawnableBase>());
        }

        private void SpawnInitialBasePlatforms()
        {
            Debug.Log("spawning initial platforms");
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

        public List<SpawnableBase> GetActivePlatforms()
        {
            return spawnedObjects;
        }

        public override void ResetService(IObjectPooler objectPooler)
        {
            base.ResetService(objectPooler);
            lastBasePlatformPosition = Vector3.zero;
            lastPlatformWidth = 0f;
            SpawnInitialBasePlatforms();
        }
    }
}
