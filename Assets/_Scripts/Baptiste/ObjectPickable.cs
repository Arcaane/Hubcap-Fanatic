using System.Collections;
using UnityEngine;

public class ObjectPickable : MonoBehaviour, IPickupable
{
    private bool isPickable = true;
    private float timeBeforePickable = 2f;
    private bool isCopHasPick = false;
    public GameObject carWhoPickObjet;

    [SerializeField] private BoxCollider bCol;
    [SerializeField] private SphereCollider sCol;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Rigidbody rb;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isCopHasPick = false;
            UIIndic.instance.EnableOrDisableDeliveryZone(true);
            transform.parent = PickableManager.Instance.carPickableSocket;
            OnPickedUp();
            PickableManager.Instance.AddPickableObject(gameObject, isCopHasPick);
            Debug.Log("Pickable by player");
        }

        if (other.gameObject.CompareTag("Enemy"))
        {
            isCopHasPick = true;
            other.gameObject.transform.GetComponent<PoliceCarBehavior>().SwapTarget(
                PoliceCarManager.Instance.policeTargetPoints
                [Random.Range(0, PoliceCarManager.Instance.policeTargetPoints.Count)], 
                true
                );
            carWhoPickObjet = other.gameObject;
            transform.parent = other.transform.gameObject.transform.GetComponent<PoliceCarBehavior>().socketPickableCop.transform;
            OnPickedUp();
            PickableManager.Instance.AddCopsWhoPickAnObject(other.gameObject);
            Debug.Log("Pickable by cops");
        }   
    }

    public void OnPickedUp()
    {
        sCol.enabled = false;
        bCol.enabled = false;
        rb.isKinematic = true;
        transform.localPosition = Vector3.zero;
    }
    
    public void OnDrop()
    {
        transform.parent = PickableManager.Instance.worldSocket;
        sCol.enabled = false;
        
        if (isCopHasPick)
        {
            carWhoPickObjet.transform.parent.GetComponent<PoliceCarBehavior>().SwapTarget(PoliceCarManager.Instance.policeTargetPoints[Random.Range(0, PoliceCarManager.Instance.policeTargetPoints.Count)], true);
            PickableManager.Instance.RemoveCopsWhoPickAnObject(carWhoPickObjet);
        }
        else
        {
            PickableManager.Instance.RemovePickableObject(gameObject, isCopHasPick);
        }
        carWhoPickObjet = null;
        UIIndic.instance.EnableOrDisableDeliveryZone();
        StartCoroutine(EnablePickupAfterDelay(timeBeforePickable));
    }

    IEnumerator EnablePickupAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        sCol.enabled = true;
        bCol.enabled = true;
        rb.isKinematic = false;
    }

    public void OnDelivered()
    {
        sCol.enabled = true;
        bCol.enabled = true;
        rb.isKinematic = false;
        meshRenderer.enabled = false;

        gameObject.GetComponent<SphereCollider>().enabled = true;
        UIIndic.instance.EnableOrDisableDeliveryZone();
        //PickableManager.Instance.ResetPickableSocketPosition();
        Destroy(this.gameObject);
    }
}