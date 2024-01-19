using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Delivery : MonoBehaviour
{
    [SerializeField] private float lifeTime = 50.0f;
    [SerializeField] private float cooldownDelivery = 4.0f;
    private bool canBeDelivered = false;
    private int deliveryCount = 0;

    public MeshRenderer boxMesh;
    public Material[] transpMat;
    public Material[] solidMat;
    public ParticleSystem[] ps;
    
    private float currentTime;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && TryGetFirstPickableObject(out GameObject firstPickableObject))
        {
            Debug.Log("Delivery done!");
            
            IPickupable pickupableComponent = firstPickableObject.GetComponent<IPickupable>();
            if (pickupableComponent != null)
            {
                pickupableComponent.OnDelivered();
                OnDeliver();
            }
            else
            {
                Debug.LogError("The first socketPickableCop object does not have IPickupable component.");
            }

            PickableManager.Instance.RemovePickableObject(0);
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

    public void CanDeliver()
    {
        canBeDelivered = true;
        var materials = boxMesh.materials;
        materials[0] = transpMat[0];
        materials[1] = transpMat[1];
        boxMesh.enabled = true;
        gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
    }

    public async void OnDeliver()
    {
        boxMesh.enabled = true;
        for (int i = 0; i < ps.Length; i++) ps[i].Play();
        canBeDelivered = false;
        var materials = boxMesh.materials;
        materials[0] = solidMat[0];
        materials[1] = solidMat[1];
        await Task.Delay(1500);
        boxMesh.enabled = false;
    }
}
