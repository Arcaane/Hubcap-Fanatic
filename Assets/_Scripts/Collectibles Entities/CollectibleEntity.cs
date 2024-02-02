using UnityEngine;

public class CollectibleEntity : MonoBehaviour
{
    public bool isTrigger;
    public CollectibleGroup group;

    private void Start()
    {
        group = transform.parent.GetComponent<CollectibleGroup>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isTrigger = true;
            group.TriggeredEntity();
            // Spawn VFX
            gameObject.SetActive(false);
        }
    }
}
