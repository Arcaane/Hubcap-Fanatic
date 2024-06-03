using HubcapManager;
using UnityEngine;

namespace HubcapPickupable {
    public class DeliveryObjectHandler : BasePickupableObject, IPoolObject {
        [Header("DESTRUCTABLE BOX")] 
        [SerializeField, Pooler] private string destructableBoxKey = null;
        [SerializeField] private float explosionForce = 400;
        [SerializeField] private float explosionRadius = 10;
        private bool hasTargetUI = false;
        
        #region POOL METHODS
        
        public void InitPoolObject() {
            UIIndicatorManager.Instance.AddUITargetToList(new UITarget {
                tr = transform
            });
            hasTargetUI = true;
        }

        public void ResetPoolObject() {
            RemoveTargetUI();
        }

        /// <summary>
        /// Remove the targetUI from the screen
        /// </summary>
        private void RemoveTargetUI() {
            if (!hasTargetUI) return; 
            UIIndicatorManager.Instance.RemoveUITargetFromList(transform);
            hasTargetUI = false;
        }
        
        #endregion POOL METHODS
        
        #region PIKCUP METOHDS
        
        /// <summary>
        /// Method called when the object is picked
        /// </summary>
        /// <param name="car"></param>
        public override void PickObject(GameObject car) {
            base.PickObject(car);
            RemoveTargetUI();
        }

        /// <summary>
        /// Destroy this delivery when a car that can destroy delivery enter in collision with this object
        /// </summary>
        public override void DestroyObject() {
            DestructableDelivery boxDelivery = PoolManager.Instance.RetrieveOrCreateObject(destructableBoxKey, transform.position, Quaternion.identity).GetComponent<DestructableDelivery>();
            boxDelivery.InitDestruction(transform.position, explosionForce, explosionRadius);
            base.DestroyObject();
        }
        
        #endregion PICKUP METHODS
    }
}