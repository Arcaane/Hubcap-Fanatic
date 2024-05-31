using Hubcap;
using UnityEngine;

namespace HubcapPickupable {
    [RequireComponent(typeof(SphereCollider), typeof(BoxCollider))]
    public class BasePickupableObject : MonoBehaviour, IPickupableObject {
        [Header("BASE PICKUPABLE DATA")] 
        [SerializeField, ReadOnly] protected GameObject owner = null;
        [SerializeField, ReadOnly] protected GameObject lastOwner = null;
        [SerializeField] protected ParticleSystem dropFX = null;
        [Space] 
        [SerializeField] private float pickupRadius = 3;
        
        private SphereCollider sphereCol = null;
        private BoxCollider boxCol = null;


#if UNITY_EDITOR
        private void OnValidate() {
            GetComponent<SphereCollider>().radius = pickupRadius;
        }
#endif
        
        protected virtual void Start() {
            sphereCol = GetComponent<SphereCollider>();
            boxCol = GetComponent<BoxCollider>();
            
            sphereCol.radius = pickupRadius;
        }

        #region PIKCUP METOHDS

        /// <summary>
        /// Method called when the object is picked by an entity
        /// </summary>
        /// <param name="car"></param>
        public virtual void PickObject(GameObject car) {
            sphereCol.enabled = false;
            boxCol.enabled = false;
            owner = car;
        } 

        /// <summary>
        /// Method called when the object is dropped on the floor
        /// </summary>
        public virtual void DropObject() {
            lastOwner = owner;
            owner = null;
            dropFX.Play();
            sphereCol.enabled = true;
            boxCol.enabled = true;
        }

        /// <summary>
        /// When the object reaches its target destination and is deliver
        /// </summary>
        public virtual void DeliverObject() { }
        
        #endregion PIKCUP METOHDS

        /// <summary>
        /// Method called when CarEntity enter or exit the trigger
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerExit(Collider other) {
            if (lastOwner == null) return;
            if (other.gameObject == lastOwner) {
                lastOwner = null;
            }
        }
    }
}