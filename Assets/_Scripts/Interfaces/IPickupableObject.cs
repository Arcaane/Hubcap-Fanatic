using HubcapPickupable;
using UnityEngine;

namespace Hubcap {
    public interface IPickupableObject {
        public void PickObject(GameObject owner);
        public void DropObject();
        public void DeliverObject(CarPickupableManager car);
        public void DestroyObject();
    }
}