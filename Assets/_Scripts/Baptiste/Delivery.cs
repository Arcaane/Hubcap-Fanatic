using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Delivery : MonoBehaviour
{
    [SerializeField] private float lifeTime = 50.0f;
    [SerializeField] private float cooldownDelivery = 4.0f;
    private bool canBeDelivered = false;
    
    private float currentTime;
    public float debugCurrentTime;

    private void Start()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (CarPickableManager.Instance != null && CarPickableManager.Instance._pickableObjects != null && CarPickableManager.Instance._pickableObjects.Count > 0)
            {
                Debug.Log("Delivery done!");
            
                // Check if the first pickable object is not null before accessing its components
                GameObject firstPickableObject = CarPickableManager.Instance._pickableObjects[0];
                if (firstPickableObject != null)
                {
                    IPickupable pickupableComponent = firstPickableObject.GetComponent<IPickupable>();
                
                    // Ensure that the pickupableComponent is not null before calling OnDelivered
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
                DeliveryRessourcesManager.Instance.SpawnRandomObject();
                ResetLifeTime();
            }
            else
            {
                Debug.Log("No delivery to do.");
            }
        }
    }

    
    void Update()
    {
        debugCurrentTime = currentTime;
        currentTime += Time.deltaTime;
        if(currentTime >= cooldownDelivery)
        {
            canBeDelivered = false;
        }
        
        if (currentTime >= lifeTime)
        {
            Debug.Log("All the members of the delivery location are dead.");    
        }
    }
    
    void ResetLifeTime()
    {
        canBeDelivered = true;
        currentTime = 0.0f;
    }
}
