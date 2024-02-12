using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class ObjectPickable : MonoBehaviour, IPickupable
{
    //public int expToGiveOnDeliver = 10;
    [Header("------------------------ Parameters ------------------------")]
    public AnimationCurve expToGiveBasedOnLevel;
    private float timeBeforePickable = 2f;
    private bool isCopHasPick = false;
    public GameObject carWhoPickObjet;
    public bool isPicked;
    public float timeBeforeDestruction;

    [Header("------------------------ Collider ------------------------")]
    [SerializeField] private BoxCollider bCol;
    [SerializeField] private SphereCollider sCol;
    [SerializeField] public Rigidbody rb;
    
    [Header("------------------------ Renderer ------------------------")]
    [SerializeField] private MeshRenderer meshRenderer;
    
    [Header("------------------------ Particle System ------------------------")]
    [SerializeField] private ParticleSystem psDrop;
    [SerializeField] private ParticleSystem psPickable;
    
    [Header("------------------------ Explosion ------------------------")]
    [SerializeField] private float explosionForce = 400;
    [SerializeField] private float explosionRadius = 10;
    [SerializeField] private float timeToDestroyAfterExplosion = 2f;
    [SerializeField] private GameObject fracturedBox;
    [SerializeField] private Rigidbody[] rbds;



    public void Start()
    {
        isPicked = false;
        sCol.enabled = true;
        bCol.enabled = true;
        rb.isKinematic = false;
        carWhoPickObjet = null;
        isPickable = true;
        psDrop.Stop(true);
        psPickable.Stop(true);
        timeBeforeDestruction = 45f;
        
        //Destruction Box : 
        fracturedBox.SetActive(false);
    }

    private void Update()
    {
        if (!isPicked)
        {
            timeBeforeDestruction -= Time.deltaTime;
            if (timeBeforeDestruction < 0)
            {
                Destroy(gameObject);
            }
        }
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
            psPickable.Play(true);
            
            //Ref to the car who pick the object
            carWhoPickObjet = other.gameObject;

            OnPickedUp();
            PickableManager.Instance.AddPickableObject(gameObject, isCopHasPick);
        }

        if (other.gameObject.CompareTag("Enemy"))
        {
            if (other.gameObject.GetComponent<PoliceCarBehavior>() == null) return;
            if (other.gameObject.GetComponent<PoliceCarBehavior>().canDestroyPickable)
            {
                DestroyBox();
                return;
            }
            isCopHasPick = true;
            //Swap target
            other.gameObject.transform.GetComponent<PoliceCarBehavior>().SwapTarget(PoliceCarManager.Instance.policeTargetPoints[Random.Range(0, PoliceCarManager.Instance.policeTargetPoints.Count)], true);

            //Ref to the car who pick the object
            carWhoPickObjet = other.gameObject;
            transform.parent = other.transform.gameObject.transform.GetComponent<PoliceCarBehavior>().socketPickableCop.transform;
            psPickable.Play(true);
            other.transform.GetComponent<PoliceCarBehavior>().objectPickable = gameObject;
            OnPickedUp();
        }   
    }

    public void OnPickedUp()
    {
        isPicked = true;
        sCol.enabled = false;
        bCol.enabled = false;
        rb.isKinematic = true;
        isPickable = false;
        transform.localPosition = Vector3.zero;

        if (CarController.instance.pickedItems.Count > 0)
        {
            for (int i = 0; i < CarController.instance.pickedItems.Count; i++)
            {
                transform.localPosition += Vector3.up * i * 2.0f;
            }
        }
        
        foreach (var t in DeliveryRessourcesManager.Instance.deliveryPoints)
        {
            t.GetComponent<Delivery>().CanDeliver();
        }
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
            PickableManager.Instance.RemovePickableObject(gameObject, isCopHasPick); //Here isCopHasPick is false
        }
        carWhoPickObjet = null;
        
        UIIndic.instance.EnableOrDisableDeliveryZone();
        foreach (var t in DeliveryRessourcesManager.Instance.deliveryPoints)
        {
            t.GetComponent<Delivery>().CantDeliver();
        }
        
        StartCoroutine(EnablePickupAfterDelay(timeBeforePickable));
    }

    IEnumerator EnablePickupAfterDelay(float delay)
    {
        isPickable = false;
        sCol.enabled = true;
        bCol.enabled = true;
        rb.isKinematic = false;
        psDrop.Play(true);
        yield return new WaitForSeconds(delay);
        meshRenderer.enabled = true;
        isPickable = true;
    }

    public void OnDelivered()
    {
        sCol.enabled = true;
        bCol.enabled = true;
        rb.isKinematic = false;
        meshRenderer.enabled = false;

        int expToAdd = 
            Mathf.RoundToInt(expToGiveBasedOnLevel.Evaluate(CarExperienceManager.Instance.playerLevel) * CarExperienceManager.Instance.nbrOfDelivery);
        CarExperienceManager.Instance.GetExp(expToAdd);
        PickableManager.Instance.RemovePickableObject(gameObject, isCopHasPick);
        
        gameObject.GetComponent<SphereCollider>().enabled = true;
        UIIndic.instance.EnableOrDisableDeliveryZone();
        Destroy(gameObject);
    }

    private void DestroyBox()
    {
        ReplaceWithFracturedBox();
        foreach (var rigidbody in rbds)
        {
            rigidbody.isKinematic = false;
            rigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius);
        } 
        DestroyObjectAsync();
    }
    
    void ReplaceWithFracturedBox()
    {
        if (fracturedBox != null)
        {
            fracturedBox.SetActive(true);
            meshRenderer.enabled = false;
            bCol.enabled = false;
            sCol.enabled = false;
            isPickable = false;
        }
    }
    
    
    async void DestroyObjectAsync()
    {
        await Task.Delay((int)(timeToDestroyAfterExplosion  * 1000)); 
        Destroy(gameObject);
    }

    
    void AddRBToArray()
    {
        rbds = GetComponentsInChildren<Rigidbody>();
    }
    
    [ContextMenu("Setup/Setup Box")]
    void SetupBox()
    {
        AddRBToArray();
    } 
    
    
    [ContextMenu("Setup/Clear")]
    void ClearArray()
    { 
        rbds = null;
    }
}