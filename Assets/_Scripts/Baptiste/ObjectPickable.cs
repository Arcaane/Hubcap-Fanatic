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
            Debug.Log("Pickable by player");
        }

        if (other.gameObject.CompareTag("Enemy"))
        {
            isCopHasPick = true;
            other.gameObject.transform.parent.GetComponent<PoliceCarBehavior>().SwapTarget(PoliceCarManager.Instance.policeTargetPoints[UnityEngine.Random.Range(0, PoliceCarManager.Instance.policeTargetPoints.Count)], true); //https://www.youtube.com/watch?v=h9kSAWqqjuw
            carWhoPickObjet = other.gameObject;
            Debug.Log("Pickable by cops");
        }   
        
        OnPickedUp();
        PickableManager.Instance.AddPickableObject(gameObject, isCopHasPick);
    }

    public void OnPickedUp()
    {
        sCol.enabled = false;
        bCol.enabled = false;
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        
        transform.parent = PickableManager.Instance.carPickableSocket;
        transform.localPosition = Vector3.zero;
        if (isCopHasPick)
        {
            meshRenderer.enabled = false;
        }
    }
    
    public void OnDrop()
    {
        transform.parent = PickableManager.Instance.worldSocket;
        
        sCol.enabled = false;
        
        if (isCopHasPick)
        {
            meshRenderer.enabled = true;
            carWhoPickObjet.transform.parent.GetComponent<PoliceCarBehavior>().SwapTarget( carWhoPickObjet.transform.parent.GetComponent<PoliceCarBehavior>().target);
        }
        PickableManager.Instance.RemoveAllPickables(isCopHasPick);
        carWhoPickObjet = null;
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
        //PickableManager.Instance.ResetPickableSocketPosition();
        Destroy(this.gameObject);
    }
}