using UnityEngine;

public class ObjectPickable : MonoBehaviour, IPickupable
{
    public void OnPickedUp()
    {
        Debug.Log("Objet récupéré !");
        Destroy(gameObject); 
    }
}