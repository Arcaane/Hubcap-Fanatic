using System;
using UnityEngine;

public class ObjectPickable : MonoBehaviour, IPickupable
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            OnPickedUp();
            CarPickableManager.Instance.AddPickableObject(this.gameObject);
        }
    }

    public void OnPickedUp()
    {
        gameObject.GetComponent<SphereCollider>().enabled = false;

        CarPickableManager.Instance.SetPickableSocketPosition(transform);
        transform.parent = CarPickableManager.Instance._pickableSocket;
    }

    public void OnDelivered()
    {
        gameObject.GetComponent<SphereCollider>().enabled = true;
        //CarPickableManager.Instance.ResetPickableSocketPosition();
        Destroy(this.gameObject);
    }
}