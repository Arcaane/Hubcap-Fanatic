using TMPro;
using UnityEngine;

public class Delivery : MonoBehaviour
{
    [SerializeField] private float lifeTime = 50.0f;
    [SerializeField] private float cooldownDelivery = 4.0f;
    private bool canBeDelivered = true;
    private int deliveryCount = 0;
    
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
        if (other.CompareTag("Player"))
        {
            CarController.instance.pickedItems[0].gameObject.GetComponent<ObjectPickable>().OnDelivered();
        }
    }
}
