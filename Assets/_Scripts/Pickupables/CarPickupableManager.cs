using System;
using System.Collections.Generic;
using System.Linq;
using HubcapCarBehaviour;
using UnityEngine;

namespace HubcapPickupable {
    [RequireComponent(typeof(SphereCollider))]
    public class CarPickupableManager : UpdatesHandler, IUpdate {
        [Header("BASE DATA")]
        [SerializeField] private CarBehaviour carOwner = null;
        [SerializeField] private Transform baseSocket = null;
        [SerializeField] private bool destroyPickupable = false;
        
        [Header("PICKUPABLES HANDLER")]
        [SerializeField, ReadOnly] private List<BasePickupableObject> ownedPickupables = new();
        [SerializeField, ReadOnly] private List<BasePickupableObject> lastPickupables = new();
        [SerializeField] private float radiusPickupable = 3f;
        [SerializeField] private float maxAnglePerPickupable = 5f;
        private List<TypeMultiplier> typeMulitpliers = new();

        [Header("COLLISIONS")] 
        [SerializeField] private int numberOfHitBeforeDrop = 3;
        [SerializeField, ReadOnly] private int currentHitleft = 0;
        
        #region INIT METHODS

        #if UNITY_EDITOR
        private void OnValidate() => GetComponent<SphereCollider>().radius = radiusPickupable;
        #endif
        
        protected override void Start() {
            base.Start();
            GetComponent<SphereCollider>().radius = radiusPickupable;
            ResetCurrentHit();
        }

        protected override void OnEnable() {
            base.OnEnable();
            carOwner.OnColliderEnter.AddListener(OnCarCollision);
        }

        protected override void OnDisable() {
            base.OnDisable();
            carOwner.OnColliderEnter.RemoveListener(OnCarCollision);
            DropAllPickupables();
        }

        #endregion INIT METHODS
        
        public void UpdateTick() => UpdatePickupablePositionBasedOnSpeed();

        #region UPDATE PICKUPABLE DATA
        
        /// <summary>
        /// Update the position and rotation of each pickup-ables
        /// </summary>
        private void UpdatePickupablePositionBasedOnSpeed() {
            foreach (BasePickupableObject pickup in ownedPickupables) {
                pickup.transform.localPosition = GetPickupableLocalPosition(ownedPickupables.IndexOf(pickup));
                pickup.transform.localRotation = Quaternion.Lerp(pickup.transform.localRotation, GetPickupableRotation(ownedPickupables.IndexOf(pickup)), Time.deltaTime);
            }
        }
        
        /// <summary>
        /// Get the localPosition for the pickup-able
        /// </summary>
        /// <param name="index">The current index of the pickup-able</param>
        /// <returns></returns>
        private Vector3 GetPickupableLocalPosition(int index) => index == 0 ? Vector3.zero : baseSocket.InverseTransformPoint(ownedPickupables[index - 1].NextSocket.position);

        /// <summary>
        /// Get the localRotation for the pickup-able
        /// </summary>
        /// <param name="index">The current index of the pickup-able</param>
        /// <returns></returns>
        private Quaternion GetPickupableRotation(int index) => Quaternion.Euler(carOwner.speedFactor * maxAnglePerPickupable * (index + 1),0,0);

        #endregion UPDATE PICKUPABLE DATA
        
        /// <summary>
        /// Add the pickup-able to the right parent, position and rotation
        /// </summary>
        /// <param name="deliveryObject"></param>
        private void AddPickupableToCar(Transform deliveryObject) {
            deliveryObject.parent = baseSocket;
            deliveryObject.localPosition = Vector3.one;
            deliveryObject.localRotation = Quaternion.identity;
        }

        #region DROP

        /// <summary>
        /// Drop all the pickup-ables that the player have
        /// </summary>
        private void DropAllPickupables() {
            foreach (BasePickupableObject pickupable in ownedPickupables) {
                pickupable.transform.parent = null;
                pickupable.DropObject();
                lastPickupables.Add(pickupable);
            }
            ownedPickupables.Clear();
        }

        /// <summary>
        /// Reset the current number of hit before dropping delivery to the based value
        /// </summary>
        private void ResetCurrentHit() => currentHitleft = numberOfHitBeforeDrop;
        
        #endregion DROP
        
        #region COLLISION METHODS
        
        /// <summary>
        /// Method called when the car enter in collision with an object
        /// </summary>
        /// <param name="col"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnCarCollision(Collision col) {
            if (!col.gameObject.CompareTags(new[] {"Enemy", "Player"})) return;
            if (ownedPickupables.Count == 0) return;

            TakeHitByOtherCar();
        }
        
        [ContextMenu("Take a hit")]
        private void TakeHitByOtherCar() {
            currentHitleft--;
            
            if (currentHitleft > 0) return;
            DropAllPickupables();
            ResetCurrentHit();
        }
        
        /// <summary>
        /// When a Pickup-able object enter in the trigger
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other) {
            if (!other.TryGetComponent(out BasePickupableObject pickupableObject)) return;
            if (pickupableObject.isOwned) return;
            if (destroyPickupable) {
                pickupableObject.DestroyObject();
                return;
            }
            if (lastPickupables.Contains(pickupableObject)) return;
            
            pickupableObject.PickObject(carOwner.gameObject);
            AddPickupableToCar(other.transform);
            ownedPickupables.Add(pickupableObject);
        }
        
        /// <summary>
        /// When a collider (pickup-able object) exit the trigger
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerExit(Collider other) {
            if (!other.TryGetComponent(out BasePickupableObject pickupableObject)) return;
            if (lastPickupables.Contains(pickupableObject)) {
                lastPickupables.Remove(pickupableObject);
            }
        }
        
        #endregion COLLISION METHODS
        
        /// <summary>
        /// Deliver all the object of the type in parameter
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool DeliverAllObjectOfType(Type type) {
            bool hasDeliverObject = false;
            foreach (BasePickupableObject pickupable in ownedPickupables.ToList()) {
                if (pickupable.GetType() != type) continue;
                
                pickupable.transform.parent = null;
                pickupable.DeliverObject(this);
                ownedPickupables.Remove(pickupable);
                hasDeliverObject = true;
            }

            return hasDeliverObject;
        }

        #region MULTIPLIER

        /// <summary>
        /// Increase the amount of a certain mutliplier
        /// </summary>
        /// <param name="T"></param>
        /// <param name="additiveAmount"></param>
        public void AddMultiplierToType(Type type, float additiveAmount) {
            foreach (TypeMultiplier typeMulitplier in typeMulitpliers) {
                if (typeMulitplier.type != type) continue;
                typeMulitplier.multiplier += additiveAmount;
                return;
            }

            CreateNewTypeMultiplier(type).multiplier += additiveAmount;
        }

        /// <summary>
        /// Get the multiplier for a specific type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public float GetMultiplierByType(Type type) {
            foreach (TypeMultiplier typeMulitplier in typeMulitpliers) {
                if (typeMulitplier.type != type) continue;
                return typeMulitplier.multiplier;
            }

            CreateNewTypeMultiplier(type);
            return 1f;
        }

        /// <summary>
        /// Create a new element in the list of TypeMultiplier
        /// </summary>
        private TypeMultiplier CreateNewTypeMultiplier(Type type) {
            typeMulitpliers.Add(new TypeMultiplier() {type = type, multiplier = 1f});
            return typeMulitpliers[^1];
        }

        #endregion
        

    }

    [Serializable]
    public class TypeMultiplier {
        public Type type = null;
        public float multiplier = 1.0f;
    }
}