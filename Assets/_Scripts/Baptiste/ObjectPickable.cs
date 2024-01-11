using System;
using System.Collections;
using UnityEngine;

public class ObjectPickable : MonoBehaviour, IPickupable
{
    private bool isPickable = true;
    private float timeBeforePickable = 2f;
    
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
        transform.parent = CarPickableManager.Instance._pickableSocket; 
    }
    
    public void OnDrop()
    {
        transform.parent = CarPickableManager.Instance.worldSocket;
        gameObject.GetComponent<SphereCollider>().enabled = false;
        CarPickableManager.Instance.RemoveAllPickables();
        StartCoroutine(EnablePickupAfterDelay(timeBeforePickable));
    }

    IEnumerator EnablePickupAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.GetComponent<SphereCollider>().enabled = true;
    }

    public void OnDelivered()
    {
        gameObject.GetComponent<SphereCollider>().enabled = true;
        //CarPickableManager.Instance.ResetPickableSocketPosition();
        Destroy(this.gameObject);
    }
}