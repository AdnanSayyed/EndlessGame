using System.Collections.Generic;
using UnityEngine;

namespace EndlessGame.Spawner
{
    public interface IPlatformSpawner : ISpawnerBase
    {
        void Initialize();
        void UpdateSpawner();
        List<Transform> GetActivePlatforms();
    }
}
