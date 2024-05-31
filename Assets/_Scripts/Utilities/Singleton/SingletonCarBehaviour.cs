using HubcapCarBehaviour;
using UnityEngine;

namespace Helper {
    public class SingletonCarBehaviour<T> : CarBehaviour where T : CarBehaviour {
        [SerializeField] protected bool dontDestroyOnLoad = false;
        private static T instance = null;
        public static T Instance => instance;

        protected virtual void Awake() {
            if (instance != null) {
                Destroy(gameObject);
                return;
            }

            instance = this as T;
            if (dontDestroyOnLoad) DontDestroyOnLoad(this);
        }
    }
}