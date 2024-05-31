using System.Collections.Generic;
using HubcapPickupable;
using UnityEngine;

namespace HubcapCarBehaviour {
    public class CarDeliveryHandler : MonoBehaviour {
        [SerializeField] private Transform deliverySocket = null;
        [SerializeField] private Vector3 deliveryScale = new Vector3(.75f, .75f, .75f);
        [Space]
        [SerializeField] private int numberOfHitBeforeDroppingDelivery = 3;
        [SerializeField, ReadOnly] private int currentNumberOfHitLeft = 0;
        [SerializeField, ReadOnly] private List<DeliveryObjectHandler> pickedDelivery = new();
        [SerializeField] private bool canDestroyDelivery = false;
        public bool CanDestroyDelivery => canDestroyDelivery;
        
        #region INIT METHODS
        
        private void OnEnable() {
            ResetCurrentHit();
            if(TryGetComponent(out CarBehaviour car)) car.OnColliderEnter.AddListener(OnPlayerEnterCollision);
        }

        private void OnDisable() {
            if(TryGetComponent(out CarBehaviour car)) car.OnColliderEnter.RemoveListener(OnPlayerEnterCollision);
            DropAllDeliveries();
        }
        
        #endregion INIT METHODS

        /// <summary>
        /// Method called when this car pickup a delivery on the road
        /// </summary>
        /// <param name="deliveryObject"></param>
        public void PickupDelivery(DeliveryObjectHandler deliveryObject) {
            pickedDelivery.Add(deliveryObject);
            deliveryObject.transform.parent = deliverySocket;
            deliveryObject.transform.localPosition = Vector3.zero;
            deliveryObject.transform.localRotation = Quaternion.identity;
            deliveryObject.transform.localScale = deliveryScale;
        }
        
        /// <summary>
        /// Method called when the player enter in collision with something
        /// </summary>
        /// <param name="col"></param>
        public void OnPlayerEnterCollision(Collision col) {
            if (!col.gameObject.CompareTags(new[] { "Enemy", "Player" })) return;
            if (pickedDelivery.Count == 0) return;
            
            currentNumberOfHitLeft--;
            
            if (currentNumberOfHitLeft > 0) return;
            DropAllDeliveries();
            ResetCurrentHit();
        }

        /// <summary>
        /// Drop all deliveries on the ground
        /// </summary>
        private void DropAllDeliveries() {
            foreach (DeliveryObjectHandler delivery in pickedDelivery) {
                delivery.transform.parent = null;
                delivery.DropObject();
                //delivery.rb.AddForce(col.contacts[0].normal.normalized * 100);
            }
            pickedDelivery.Clear();
        }
        
        /// <summary>
        /// Reset the current number of hit before dropping delivery to the based value
        /// </summary>
        private void ResetCurrentHit() => currentNumberOfHitLeft = numberOfHitBeforeDroppingDelivery;
    }
}