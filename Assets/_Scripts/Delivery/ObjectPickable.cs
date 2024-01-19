using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectPickable : MonoBehaviour, IPickupable
{
    //public int expToGiveOnDeliver = 10;
    public AnimationCurve expToGiveBasedOnLevel;
    
    private float timeBeforePickable = 2f;
    private bool isCopHasPick = false;
    public GameObject carWhoPickObjet;

    [SerializeField] private BoxCollider bCol;
    [SerializeField] private SphereCollider sCol;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] public Rigidbody rb;
    [SerializeField] private ParticleSystem ps;

    public void Start()
    {
        sCol.enabled = true;
        bCol.enabled = true;
        rb.isKinematic = false;
        carWhoPickObjet = null;
        isPickable = true;
    }

    public bool isPickable;
    private void OnTriggerEnter(Collider other)
    {
        if (!isPickable) return;
        
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
            //Swap target
            other.gameObject.transform.GetComponent<PoliceCarBehavior>().SwapTarget(PoliceCarManager.Instance.policeTargetPoints[Random.Range(0, PoliceCarManager.Instance.policeTargetPoints.Count)], true);
            //Ref to the car who pick the object
            carWhoPickObjet = other.gameObject;
            transform.parent = other.transform.gameObject.transform.GetComponent<PoliceCarBehavior>().socketPickableCop.transform;
            other.transform.GetComponent<PoliceCarBehavior>().objectPickable = gameObject;
            OnPickedUp();
            //PickableManager.Instance.AddCopsWhoPickAnObject(other.gameObject);
            Debug.Log("Pickable by cops");
        }   
    }

    public void OnPickedUp()
    {
        sCol.enabled = false;
        bCol.enabled = false;
        rb.isKinematic = true;
        transform.localPosition = Vector3.zero;
        isPickable = false;
    }
    
    public void OnDrop()
    {
        transform.parent = PickableManager.Instance.worldSocket; //Switch to World Socket
        sCol.enabled = false;
        
        if (isCopHasPick)
        {
            carWhoPickObjet.transform.GetComponent<PoliceCarBehavior>().
                SwapTarget(PoliceCarManager.Instance.policeTargetPoints[Random.Range(0, PoliceCarManager.Instance.policeTargetPoints.Count)]);
        }
        else
        {
            PickableManager.Instance.RemovePickableObject(gameObject, isCopHasPick);
        }
        carWhoPickObjet = null;
        
        UIIndic.instance.EnableOrDisableDeliveryZone();
        foreach (var t in DeliveryRessourcesManager.Instance.deliveryPoints)
        {
            t.GetComponent<Delivery>().CanDeliver();
        }
        
        StartCoroutine(EnablePickupAfterDelay(timeBeforePickable));
    }

    IEnumerator EnablePickupAfterDelay(float delay)
    {
        isPickable = false;
        sCol.enabled = true;
        bCol.enabled = true;
        rb.isKinematic = false;
        ps.Play();
        yield return new WaitForSeconds(delay);
        isPickable = true;
    }

    public void OnDelivered()
    {
        sCol.enabled = true;
        bCol.enabled = true;
        rb.isKinematic = false;
        meshRenderer.enabled = false;
        
        CarExperienceManager.Instance.GetExp(Mathf.RoundToInt(expToGiveBasedOnLevel.Evaluate(CarExperienceManager.Instance.playerLevel)));
        
        gameObject.GetComponent<SphereCollider>().enabled = true;
        UIIndic.instance.EnableOrDisableDeliveryZone();
        Destroy(gameObject);
    }
}