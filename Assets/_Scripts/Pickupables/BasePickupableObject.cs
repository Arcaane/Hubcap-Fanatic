using Hubcap;
using HubcapManager;
using UnityEngine;

namespace HubcapPickupable {
    [RequireComponent(typeof(BoxCollider), typeof(Rigidbody))]
    public class BasePickupableObject : MonoBehaviour, IPickupableObject {
        [Header("PICKUPABLE DATA")]
        [SerializeField, ReadOnly] protected GameObject owner = null;
        [SerializeField] private Transform nextSocket = null;
        [SerializeField] protected ParticleSystem dropFX = null;
        [Space]
        [SerializeField] private Vector3 scaleWhenPicked = new();
        [SerializeField] private float projectionStrength = 10;
        public Transform NextSocket => nextSocket;
        public bool isOwned => owner != null;
        private BoxCollider boxCol = null;
        private Rigidbody rb = null;
        
        protected virtual void Start() {
            boxCol = GetComponent<BoxCollider>();
            rb = GetComponent<Rigidbody>();
        }

        #region PIKCUP METOHDS

        /// <summary>
        /// Method called when the object is picked by an entity
        /// </summary>
        /// <param name="car"></param>
        public virtual void PickObject(GameObject car) {
            transform.localScale = scaleWhenPicked;
            boxCol.enabled = false;
            rb.isKinematic = true;
            owner = car;
        }

        /// <summary>
        /// Method called when the object is dropped on the floor
        /// </summary>
        public virtual void DropObject() {
            transform.localScale = Vector3.one;
            boxCol.enabled = true;
            rb.isKinematic = false;
            owner = null;
            dropFX.Play();
            ApplyForceInRandomDir();
        }

        /// <summary>
        /// When the object reaches its target destination and is deliver
        /// </summary>
        public virtual void DeliverObject() { }

        /// <summary>
        /// When the object need to be destroyed
        /// </summary>
        public virtual void DestroyObject() => PoolManager.Instance.RemoveObjectFromScene(gameObject);

        #endregion PIKCUP METOHDS

        /// <summary>
        /// Apply a force in a random direction when dropped
        /// </summary>
        private void ApplyForceInRandomDir() {
            Vector3 rndDir =Quaternion.Euler(0,Random.Range(0,360),0) * transform.forward;
            rb.AddForce(rndDir * projectionStrength, ForceMode.VelocityChange);
        }
    }
}