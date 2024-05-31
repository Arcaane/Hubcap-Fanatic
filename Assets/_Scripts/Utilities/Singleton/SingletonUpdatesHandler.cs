using UnityEngine;

namespace Helper {
    public class SingletonUpdatesHandler<T> : UpdatesHandler where T : UpdatesHandler {
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