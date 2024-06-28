using EndlessGame.ObjectPool;
using EndlessGame.Spawnable;
using UnityEngine;

namespace EndlessGame.Spawner
{
    public class ObstacleSpawner : SpawnerBase, IObstacleSpawner
    {
        [SerializeField] float obstacleSpawnChance = 0.3f;
        [SerializeField] float minObstacleDistance = 5f;
        [SerializeField] float spawnDelay = 5f;

        private float startTime;

        void Start()
        {
            startTime = Time.time;
        }

        public override void Spawn(GameObject platform, IObjectPooler objectPooler, ref float lastSpawnX)
        {
            if (Time.time - startTime < spawnDelay)
            {
                return;
            }

            float platformX = platform.transform.position.x;
            float platformWidth = platform.GetComponent<Platform>().Length;

            if (Random.value < obstacleSpawnChance && platformX - lastSpawnX >= minObstacleDistance)
            {
                Obstacle obstacleToSpawn = (Obstacle)allPrefabs[Random.Range(0, allPrefabs.Length)];
                float yOffset = obstacleToSpawn.SpawnYOffset;

                Vector3 obstaclePosition = new Vector3(
                    platform.transform.position.x,
                    platform.transform.position.y + yOffset,
                    platform.transform.position.z
                );
                GameObject obstacle = objectPooler.SpawnFromPool(obstacleToSpawn.SpawnableTag, obstaclePosition, Quaternion.identity);

                spawnedObjects.Add(obstacle.GetComponent<SpawnableBase>());

                lastSpawnX = platformX;
            }
        }

        public override void ResetService(IObjectPooler objectPooler)
        {
            base.ResetService(objectPooler);
            startTime = Time.time;
        }

    }
}
