using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace HubcapInterface.DoTween {
    public class DoTweenEffect : MonoBehaviour {
        protected List<Tweener> tweeners = new();
        
        public virtual void StartEffect(bool loop) {
        }

        public virtual void StopEffect() {
            foreach (Tweener tweener in tweeners) {
                tweener.Rewind();
            }
        }
    }
}