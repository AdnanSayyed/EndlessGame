using EndlessGame.ObjectPool;
using UnityEngine;

namespace EndlessGame.Spawner
{
    public interface IObstacleSpawner : ISpawnerBase { }

    public interface ICollectableSpawner : ISpawnerBase { }

    public interface ISpawnerBase
    {
        void Spawn(GameObject platform, IObjectPooler objectPooler, ref float lastSpawnX);
        void ReturnToPool(IObjectPooler objectPooler);
        void CheckAndReturnToPool(IObjectPooler objectPooler, Transform playerTransform, float offset = 15f);

        void ResetService(IObjectPooler objectPooler);


    }

    public interface ISpawner
    {
        void Initialize();
        void UpdateSpawner();
    }


}