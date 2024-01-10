using System;
using System.Collections;
using UnityEngine;

public class ObjectPickable : MonoBehaviour, IPickupable
{
    private bool isPickable = true;
    
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
        isPickable = false; 
    }
    
    public void OnDrop()
    {
        transform.parent = CarPickableManager.Instance.worldSocket;
        CarPickableManager.Instance.RemovePickableObject(0);
        StartCoroutine(EnablePickupAfterDelay(2f));
    }

    IEnumerator EnablePickupAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.GetComponent<SphereCollider>().enabled = true;
        isPickable = true;
    }

    public void OnDelivered()
    {
        gameObject.GetComponent<SphereCollider>().enabled = true;
        //CarPickableManager.Instance.ResetPickableSocketPosition();
        Destroy(this.gameObject);
    }
}