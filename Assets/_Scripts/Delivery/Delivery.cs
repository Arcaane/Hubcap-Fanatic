using TMPro;
using UnityEngine;

public class Delivery : MonoBehaviour
{
    [SerializeField] private float lifeTime = 50.0f;
    [SerializeField] private float cooldownDelivery = 4.0f;
    private bool canBeDelivered = true;
    private int deliveryCount = 0;
    
    [SerializeField] private TextMeshPro deliveryCountText;
    
    private float currentTime;

    private void Start()
    {
        canBeDelivered = true;
    }

    private void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime >= cooldownDelivery)
        {
            currentTime = cooldownDelivery;
            canBeDelivered = true;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && TryGetFirstPickableObject(out GameObject firstPickableObject))
        {
            Debug.Log("Delivery done!");
            
            IPickupable pickupableComponent = firstPickableObject.GetComponent<IPickupable>();
            if (pickupableComponent != null)
            {
                pickupableComponent.OnDelivered();
            }
            else
            {
                Debug.LogError("The first socketPickableCop object does not have IPickupable component.");
            }

            PickableManager.Instance.RemovePickableObject(0);
            //DeliveryResourcesManager.Instance.SpawnRandomObject();
            ResetLifeTime();
        }
    }
    
    private bool TryGetFirstPickableObject(out GameObject firstPickableObject)
    {
        firstPickableObject = null;

        if (PickableManager.Instance != null && PickableManager.Instance.car.pickedItems != null && PickableManager.Instance.car.pickedItems.Count > 0)
        {
            firstPickableObject = PickableManager.Instance.car.pickedItems[0];
        }

        return firstPickableObject != null;
    }
    
    private void ResetLifeTime()
    {
        canBeDelivered = false;
        currentTime = 0.0f;
    }
}
