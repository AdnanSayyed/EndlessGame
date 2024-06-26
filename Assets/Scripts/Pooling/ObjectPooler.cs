using EndlessGame.Service;
using System.Collections.Generic;
using UnityEngine;

namespace EndlessGame.ObjectPool
{
    public class ObjectPooler : MonoBehaviour, IObjectPooler
    {
        [System.Serializable]
        public class Pool
        {
            public string tag;
            public GameObject prefab;
            public int size;
        }

        public List<Pool> pools;
        private Dictionary<string, Queue<GameObject>> poolDictionary;
        public static ObjectPooler Instance;

        void Awake()
        {
            Instance = this;

            poolDictionary = new Dictionary<string, Queue<GameObject>>();

            foreach (Pool pool in pools)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();

                for (int i = 0; i < pool.size; i++)
                {
                    GameObject obj = Instantiate(pool.prefab, transform);
                    obj.SetActive(false);
                    objectPool.Enqueue(obj);
                }

                poolDictionary.Add(pool.tag, objectPool);
            }

            ServiceLocator.RegisterService<IObjectPooler>(this);
        }

        public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
                return null;
            }

            GameObject objectToSpawn = poolDictionary[tag].Dequeue();
            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;

            poolDictionary[tag].Enqueue(objectToSpawn);

            return objectToSpawn;
        }

        public void ReturnToPool(string tag, GameObject objectToReturn)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
                return;
            }

            objectToReturn.SetActive(false);
            poolDictionary[tag].Enqueue(objectToReturn);
        }

        public void ResetService()
        {
            foreach (var pool in pools)
            {
                if (poolDictionary.ContainsKey(pool.tag))
                {
                    Queue<GameObject> objectPool = poolDictionary[pool.tag];
                    List<GameObject> activeObjects = new List<GameObject>();

                    // Dequeue all objects
                    while (objectPool.Count > 0)
                    {
                        GameObject obj = objectPool.Dequeue();
                        if (obj.activeSelf)
                        {
                            activeObjects.Add(obj);
                        }
                    }

                    // Deactivate and re-enqueue them
                    foreach (GameObject obj in activeObjects)
                    {
                        obj.SetActive(false);
                        objectPool.Enqueue(obj);
                    }

                    // Ensure the pool has the right amount of objects
                    while (objectPool.Count < pool.size)
                    {
                        GameObject obj = Instantiate(pool.prefab, transform);
                        obj.SetActive(false);
                        objectPool.Enqueue(obj);
                    }

                    poolDictionary[pool.tag] = objectPool;
                }
            }
        }
    }
}
