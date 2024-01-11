using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Delivery : MonoBehaviour
{
    [SerializeField] private float lifeTime = 50.0f;
    [SerializeField] private float cooldownDelivery = 4.0f;
    private bool canBeDelivered = true;
    
    private float currentTime;
    public float debugCurrentTime;


    private void Start()
    {
        canBeDelivered = true;
    }

    void Update()
    {
        debugCurrentTime = currentTime; //Debug Time
        currentTime += Time.deltaTime;
        if(currentTime >= cooldownDelivery)
        {
            canBeDelivered = true;
            Debug.Log("Delivery can be done :" + gameObject.name);
        }
        
        if (currentTime >= lifeTime)
        {
            currentTime = 0f;
            Debug.Log("All the members of the delivery location are dead.");    
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (CarPickableManager.Instance != null && CarPickableManager.Instance._pickableObjects != null && CarPickableManager.Instance._pickableObjects.Count > 0)
            {
                Debug.Log("Delivery done!");
            
                GameObject firstPickableObject = CarPickableManager.Instance._pickableObjects[0];
                if (firstPickableObject != null)
                {
                    IPickupable pickupableComponent = firstPickableObject.GetComponent<IPickupable>();
                
                    if (pickupableComponent != null)
                    {
                        pickupableComponent.OnDelivered();
                    }
                    else
                    {
                        Debug.LogError("The first pickable object does not have IPickupable component.");
                    }
                }
                else
                {
                    Debug.LogError("The first pickable object is null.");
                }

                CarPickableManager.Instance.RemovePickableObject(0);
                //DeliveryRessourcesManager.Instance.SpawnRandomObject();
                ResetLifeTime();
            }
            else
            {
                Debug.Log("No delivery to do.");
            }
        }
    }

    
    void ResetLifeTime()
    {
        canBeDelivered = false;
        currentTime = 0.0f;
    }
}
