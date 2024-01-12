using System;
using System.Collections;
using UnityEngine;

public class ObjectPickable : MonoBehaviour, IPickupable
{
    private bool isPickable = true;
    private float timeBeforePickable = 2f;
    private bool isCopHasPick = false;
    public GameObject carWhoPickObjet;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isCopHasPick = false;
            Debug.Log("Pickable by player");
        }

        if (other.gameObject.CompareTag("Cone"))
        {
            isCopHasPick = true;
            Debug.Log("Pickable by cops");
        }   
        carWhoPickObjet = other.gameObject;
        OnPickedUp();
        PickableManager.Instance.AddPickableObject(this.gameObject, isCopHasPick);
    }

    public void OnPickedUp()
    {
        gameObject.GetComponent<SphereCollider>().enabled = false;
        transform.parent = PickableManager.Instance.carPickableSocket;
        if (isCopHasPick)
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }
    
    public void OnDrop()
    {
        transform.parent = PickableManager.Instance.worldSocket;
        gameObject.GetComponent<SphereCollider>().enabled = false;
        if (isCopHasPick)
        {
            gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
        PickableManager.Instance.RemoveAllPickables(isCopHasPick);
        carWhoPickObjet = null;
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
        //PickableManager.Instance.ResetPickableSocketPosition();
        Destroy(this.gameObject);
    }
}