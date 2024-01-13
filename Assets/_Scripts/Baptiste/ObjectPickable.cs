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

        if (other.gameObject.CompareTag("Enemy"))
        {
            isCopHasPick = true;
            other.gameObject.transform.root.GetComponent<PoliceCarBehavior>().SwapTarget(PoliceCarManager.Instance.policeTargetPoints[UnityEngine.Random.Range(0, PoliceCarManager.Instance.policeTargetPoints.Count)], true); //https://www.youtube.com/watch?v=h9kSAWqqjuw
            carWhoPickObjet = other.gameObject;
            Debug.Log("Pickable by cops");
        }   
        OnPickedUp();
        PickableManager.Instance.AddPickableObject(this.gameObject, isCopHasPick);
    }

    public void OnPickedUp()
    {
        gameObject.GetComponent<SphereCollider>().enabled = false;
        transform.parent = PickableManager.Instance.carPickableSocket;
        transform.localPosition = Vector3.zero;
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
            carWhoPickObjet.transform.root.GetComponent<PoliceCarBehavior>().SwapTarget( carWhoPickObjet.transform.root.GetComponent<PoliceCarBehavior>().target);
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