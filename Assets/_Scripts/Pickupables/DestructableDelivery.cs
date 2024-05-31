using System.Collections.Generic;
using System.Linq;
using HubcapManager;
using UnityEngine;

namespace HubcapPickupable {
    public class DestructableDelivery : UpdatesHandler, IPoolObject, IUpdate {
        [SerializeField] private float destructionDuration = 10f; 
        [SerializeField, ReadOnly] private List<Rigidbody> boxRigidbodies = new();
        [SerializeField, ReadOnly] private List<Transform> boxTransforms = new();
        [SerializeField, ReadOnly] private List<Vector3> startPositions = new();
        [SerializeField, ReadOnly] private List<Quaternion> startRotations = new();
        private float durationLeft = 0;

        #if UNITY_EDITOR
        [ContextMenu("Init Data")]
        private void InitDataInEditor() {
            boxRigidbodies.Clear();
            boxTransforms.Clear();
            startPositions.Clear();
            startRotations.Clear();
            
            boxRigidbodies = GetComponentsInChildren<Rigidbody>().ToList();
            foreach (Rigidbody rb in boxRigidbodies) {
                startPositions.Add(rb.transform.localPosition);
                startRotations.Add(rb.transform.localRotation);
                boxTransforms.Add(rb.transform);
            }
        }
        #endif
        
        
        #region POOL METHODS
        public void InitPoolObject() {
            durationLeft = destructionDuration;
            foreach (Rigidbody rb in boxRigidbodies) {
                rb.isKinematic = false;
            }
        }

        public void ResetPoolObject() => ResetBoxData();

        #endregion DESTRUCTION METHODS
        
        #region DESTRUCTION METHODS
        
        /// <summary>
        /// Initialize the destruction of the object
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="explosionForce"></param>
        /// <param name="explosionRadius"></param>
        public void InitDestruction(Vector3 pos, float explosionForce, float explosionRadius) {
            foreach (Rigidbody rb in boxRigidbodies) {
                rb.AddExplosionForce(explosionForce, pos, explosionRadius);
            }
        }

        /// <summary>
        /// Reset the position of all rigidbody inside the list of rbs
        /// </summary>
        private void ResetBoxData() {
            for (var index = 0; index < boxRigidbodies.Count; index++) {
                boxTransforms[index].localPosition = startPositions[index];
                boxTransforms[index].localRotation = startRotations[index];
                boxTransforms[index].localScale = Vector3.one;
                boxRigidbodies[index].isKinematic = true;
            }
        }
        
        #endregion DESTRUCTION METHODS

        public void UpdateTick() {
            durationLeft -= Time.deltaTime;

            float scale = durationLeft / destructionDuration;
            foreach (Transform tr in boxTransforms) {
                tr.localScale = new Vector3(scale, scale, scale);
            }

            if (durationLeft <= 0) {
                PoolManager.Instance.RemoveObjectFromScene(gameObject);
            }
        }
    }
}