using UnityEngine;

namespace EndlessGame.ObjectPool
{
    public interface IObjectPooler
    {
        GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation);
        void ReturnToPool(string tag, GameObject objectToReturn);

        void ResetService();
    }

}