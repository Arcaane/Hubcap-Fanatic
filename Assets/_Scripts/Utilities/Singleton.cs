using UnityEngine;

namespace Helper {
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
        [SerializeField] protected bool dontDestroyOnLoad = false;
        private static T instance = null;
        public static T Instance => instance;

        private void Awake() {
            if (instance != null) {
                Destroy(gameObject);
                return;
            }

            instance = this as T;
            if(dontDestroyOnLoad) DontDestroyOnLoad(this);
            AwakeContinue();
        }

        /// <summary>
        /// Method which is called inside the awake method
        /// </summary>
        protected virtual void AwakeContinue() {}
    }
}
