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
        if (other.CompareTag("Player"))
        {
            if (CarController.instance.pickedItems.Count <= 0) return;
            for (int i = CarController.instance.pickedItems.Count - 1; i >= 0; i--)
            {
                Debug.Log("Delivered");
                CarController.instance.pickedItems[i].gameObject.GetComponent<ObjectPickable>().OnDelivered();
                OnDeliver();
            }
        }
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
        if (boxMesh != null) boxMesh.enabled = true;
        for (int i = 0; i < ps.Length; i++) ps[i].Play();
        canBeDelivered = false;
        var materials = boxMesh.materials;
        materials[0] = solidMat[0];
        materials[1] = solidMat[1];
        await Task.Delay(1500);
        if (boxMesh != null) boxMesh.enabled = false;
    }
}
