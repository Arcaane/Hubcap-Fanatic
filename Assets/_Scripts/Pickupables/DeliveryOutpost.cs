using UnityEngine;

namespace HubcapPickupable {
    [RequireComponent(typeof(BoxCollider))]
    public class DeliveryOutpost : MonoBehaviour {
        [SerializeField] private ParticleSystem deliveryDroped = null;
        
        private void OnTriggerEnter(Collider other) {
            if (!other.TryGetComponent(out CarPickupableManager car)) return;
            if (!car.DeliverAllObjectOfType(typeof(DeliveryObjectHandler))) return;
            deliveryDroped.Play();
        }
    }
}