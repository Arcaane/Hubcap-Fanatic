using UnityEngine;

namespace HubcapManager.Pooler {
    public class EffectPoolCallback : MonoBehaviour {
        /// <summary>
        /// Method called when ParticleSystem end its animation
        /// </summary>
        public void OnParticleSystemStopped() {
            PoolManager.Instance.RemoveObjectFromScene(gameObject);
        }
    }
}