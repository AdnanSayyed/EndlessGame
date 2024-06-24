using UnityEngine;
using EndlessGame.ObjectPool;
using EndlessGame.Service;
using EndlessGame.Player;

namespace EndlessGame.Spawner
{
    public class Spawner : MonoBehaviour, ISpawner
    {
        private IPlatformSpawner platformSpawner;
        private IObstacleSpawner obstacleSpawner;
        private ICollectableSpawner collectableSpawner;

        private IObjectPooler objectPooler;
        private Transform playerTransform;
        private float lastSpawnX;

        public void Initialize()
        {
            platformSpawner = ServiceLocator.GetService<IPlatformSpawner>();
            obstacleSpawner = ServiceLocator.GetService<IObstacleSpawner>();
            collectableSpawner = ServiceLocator.GetService<ICollectableSpawner>();
            objectPooler = ServiceLocator.GetService<IObjectPooler>();

            playerTransform = ServiceLocator.GetService<IPlayerController>().GetTransform();

            platformSpawner.Initialize();
        }

        public void UpdateSpawner()
        {
            platformSpawner.UpdateSpawner();
            UpdateObstaclesAndCollectables();
        }

        private void UpdateObstaclesAndCollectables()
        {
            foreach (Transform platform in platformSpawner.GetActivePlatforms())
            {
                obstacleSpawner.Spawn(platform.gameObject, objectPooler, ref lastSpawnX);
                collectableSpawner.Spawn(platform.gameObject, objectPooler, ref lastSpawnX);
            }

            obstacleSpawner.CheckAndReturnToPool(objectPooler, playerTransform);
            collectableSpawner.CheckAndReturnToPool(objectPooler, playerTransform);
        }
    }
}