using UnityEngine;

public interface ISpawner
{
    void Spawn(GameObject gameObject, ObjectPooler objectPooler, ref float lastXSpawnPos);

    void ReturnToPool(ObjectPooler objectPooler);
}
