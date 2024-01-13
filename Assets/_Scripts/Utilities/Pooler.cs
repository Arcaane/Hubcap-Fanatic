using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace ManagerNameSpace
{
    public class Pooler : MonoBehaviour
    {
        public static Pooler instance;
        private Dictionary<Key, Pool> pools = new Dictionary<Key, Pool>();
        [SerializeField] private List<PoolKey> poolKeys = new List<PoolKey>();
        private Component objectInstance;

        void Start()
        {
            instance = this;
            InitPools();
        }

        /// <summary>
        /// Get an instance of a Transform or ParticleSystem from pool
        /// </summary>
        public Component SpawnInstance(Key key, Vector3 position, Quaternion rotation)
        {
            if (pools[key].queue.Count == 0)
            {
                Debug.LogWarning("pool of " + key + "is empty");
                AddInstance(pools[key]);
            }

            objectInstance = pools[key].queue.Dequeue();
            objectInstance.gameObject.SetActive(true);
            objectInstance.transform.SetPositionAndRotation(position, rotation);
            return objectInstance;
        }
        
        /// <summary>
        /// Get an instance of a Transform or ParticleSystem from pool
        /// </summary>
        public Component SpawnTemporaryInstance(Key key, Vector3 position, Quaternion rotation, float time = 0)
        {
            if (pools[key].queue.Count == 0)
            {
                Debug.LogWarning("pool of " + key + "is empty");
                AddInstance(pools[key]);
            }

            objectInstance = pools[key].queue.Dequeue();
            objectInstance.gameObject.SetActive(true);
            objectInstance.transform.SetPositionAndRotation(position, rotation);
            DestroyInstance(key, objectInstance, time);
            return objectInstance;
        }

        /// <summary>
        /// Destroy Instance from Pool
        /// </summary>
        public async void DestroyInstance(Key key, Component obj, float time = 0)
        {
            if (time > 0)
            {
                await Task.Delay(Mathf.RoundToInt(time * 1000));
            }

            if (obj != null)
            {
                pools[key].queue.Enqueue(obj);
                obj.transform.parent = transform;
                obj.gameObject.SetActive(false);
            }
        }

        private void AddInstance(Pool pool)
        {
            switch (pool.returnType)
            {
                case ReturnType.Transform:
                    objectInstance = Instantiate(pool.prefab, transform).transform;
                    break;
                case ReturnType.ParticleSystem:
                    objectInstance = Instantiate(pool.prefab, transform).GetComponent<ParticleSystem>();
                    break;
            }

            objectInstance.gameObject.SetActive(false);
            pool.queue.Enqueue(objectInstance);
        }

        private void InitPools()
        {
            int i;

            for (i = 0; i < poolKeys.Count; i++)
            {
                pools.Add(poolKeys[i].key, poolKeys[i].pool);
            }

            foreach (var pool in pools)
            {
                for (i = 0; i < pool.Value.numberOfInstances; i++)
                {
                    AddInstance(pool.Value);
                }

                //Debug.Log(pool.Key + " Has " + pool.Value.queue.Count);
            }
        }
    }

    [Serializable]
    public class Pool
    {
        public GameObject prefab;
        public int numberOfInstances;
        public Queue<Component> queue = new Queue<Component>();
        public ReturnType returnType;
    }

    [Serializable]
    public class PoolKey
    {
        public Key key;
        public Pool pool;
    }

    public enum Key
    {
        FX_Puddle,
        FX_FluidSplash,
        OBJ_Foddler,
        OBJ_BerserkEnemy,
        OBJ_DriveByEnemy
    }

    public enum ReturnType
    {
        Transform,
        ParticleSystem
    }
}