using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Delivery : MonoBehaviour
{
    private bool canBeDelivered = false;
 
    
    [Header("--------- PARTICULES ---------")]
    public GameObject deliveryZone;
    public ParticleSystem ps;
    
    private float currentTime;

    private void Update()
    {
        if (CarController.instance.pickedItems.Count <= 0)
        {
            CantDeliver();
        }
        else
        {
            CanDeliver();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (CarController.instance.pickedItems.Count <= 0) return;
            for (int i = CarController.instance.pickedItems.Count - 1; i >= 0; i--)
            {
                CarController.instance.pickedItems[i].gameObject.GetComponent<ObjectPickable>().OnDelivered();
                OnDeliver();
            }
        }
    }
    
    public void CanDeliver()
    {
        canBeDelivered = true;
        deliveryZone.SetActive(true);
    }

    public void CantDeliver()
    {
        canBeDelivered = false;
        deliveryZone.SetActive(false);
    }

    public async void OnDeliver()
    {
        if (deliveryZone != null) deliveryZone.SetActive(false);
        CarExperienceManager.Instance.nbrOfDelivery += 0.1f;
        canBeDelivered = false;
        ps.gameObject.SetActive(true);
        ps.Play();
        await Task.Delay(1500);
        ps.gameObject.SetActive(false);
        ps.Stop();
    }
}