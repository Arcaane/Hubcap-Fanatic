using System.Collections.Generic;
using System.Linq;
using Helper;
using UnityEngine;

namespace HubcapManager {
    public class PoolManager : Singleton<PoolManager> {
        [SerializeField] private List<PoolData> poolDatas = new();
        private Dictionary<string, PoolData> pools = new();


        protected override void Awake() {
            base.Awake();
            InitPools();
        }

        /// <summary>
        /// Initialize all pools
        /// </summary>
        private void InitPools() {
            foreach (PoolData pool in poolDatas) {
                if (pools.ContainsKey(pool.poolKey)) {
                    Debug.LogError($"This name was already used by a previous PoolData : {pool.poolKey}");
                    return;
                }

                pool.parentTransform = new GameObject(pool.prefab.name).transform;
                pool.parentTransform.parent = transform;
                pool.parentTransform.name = $"{pool.prefab.name}_POOL";
                pools.Add(pool.poolKey, pool);

                for (int i = 0; i < pool.amountToSpawnAtStart; i++) SpawnInstancesOfPool(pool);
            }
        }
        
        #region POOL METHODS
        
        /// <summary>
        /// Spawn the amount of gameObject based on the parameter of the pool
        /// </summary>
        /// <param name="pool"></param>
        private GameObject SpawnInstancesOfPool(PoolData pool) {
            GameObject objectInstance = Instantiate(pool.prefab, Vector3.zero, Quaternion.identity, pool.parentTransform);
            DisableObject(objectInstance);
            pool.poolObjects.Add(objectInstance);
            return objectInstance;
        }

        /// <summary>
        /// Get a specific gameObject from the pool based on a key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public GameObject RetrieveOrCreateObject(string key, Vector3 position, Quaternion rotation) {
            if(!pools.ContainsKey(key)) Debug.LogError($"there is no dictionnary containing the key : {key}");
            
            GameObject objectToActivate = null;
            objectToActivate = HasAtLeastOneInactiveObject(pools[key].poolObjects) ? GetFirstInactiveObject(key) : SpawnInstancesOfPool(pools[key]);
            objectToActivate.transform.SetPositionAndRotation(position, rotation);
            EnableObject(objectToActivate);
            
            return objectToActivate;
        }

        /// <summary>
        /// Remove the object from scene by disabling it and reset its position and rotation
        /// </summary>
        /// <param name="gam"></param>
        public void RemoveObjectFromScene(GameObject gam) {
            DisableObject(gam);
        }
        
        /// <summary>
        /// Disable the object in parameter
        /// </summary>
        /// <param name="gam"></param>
        private void DisableObject(GameObject gam) {
            if(gam.TryGetComponent(out IPoolObject poolObject)) poolObject.ResetPoolObject(); 
            gam.SetActive(false);
            gam.transform.position = Vector3.zero;
            gam.transform.rotation = Quaternion.identity;
        }

        /// <summary>
        /// Enable the object in parameter
        /// </summary>
        /// <param name="gam"></param>
        private void EnableObject(GameObject gam) {
            if(gam.TryGetComponent(out IPoolObject poolObject)) poolObject.InitPoolObject();
            gam.SetActive(true);
        }
        
        #endregion POOL METHODS
        
        #region HELPER
        
        /// <summary>
        /// Return all the current key from the PoolManager
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllPoolKeys() => poolDatas.Select(pool => pool.poolKey).ToList();
        /// <summary>
        /// Get the amount of inactive objects in a list of gameObject
        /// </summary>
        /// <param name="objects"></param>
        /// <returns></returns>
        private bool HasAtLeastOneInactiveObject(List<GameObject> objects) => objects.Count(gam => !gam.activeSelf) > 0;
        /// <summary>
        /// Get the first inactive object from the pool
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private GameObject GetFirstInactiveObject(string key) => pools[key].poolObjects.FirstOrDefault(gam => !gam.activeSelf);

        /// <summary>
        /// Reorder the list by name
        /// </summary>
        public void ReorderListByName() => poolDatas = poolDatas.OrderBy(obj => obj.poolKey).ToList();

        #endregion HELPER
    }

    [System.Serializable]
    public class PoolData {
        public string poolKey = "";
        public GameObject prefab = null;
        public int amountToSpawnAtStart = 0;
        public Transform parentTransform = null;
        public List<GameObject> poolObjects = new();
    }
}