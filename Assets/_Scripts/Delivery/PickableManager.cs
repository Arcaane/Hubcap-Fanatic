using System.Collections.Generic;
using UnityEngine;

public class PickableManager : MonoBehaviour
{
    private static PickableManager _instance;
    public static PickableManager Instance => _instance;

    [Header("Socket to take the object")] public Transform carPickableSocket;
    public Transform worldSocket;

    [Header("Object Possess")] 
    public List<GameObject> copsWhoPickAnObject = new();
    public CarController car;


    public void Start()
    {
        _instance = this;
        car = CarController.instance;
    }
    
    public void AddCopsWhoPickAnObject(GameObject pickableObject)
    {
        copsWhoPickAnObject.Add(pickableObject);
    }
    
    public void RemoveCopsWhoPickAnObject(GameObject pickableObject)
    {
        copsWhoPickAnObject.Remove(pickableObject);
        copsWhoPickAnObject = null;
    }

    public void AddPickableObject(GameObject pickableObject, bool isCopsPickable = false)
    {
        (isCopsPickable ? copsWhoPickAnObject : car.pickedItems).Add(pickableObject);
    }

    public void RemovePickableObject(GameObject pickableObject, bool isCopsPickable = false)
    {
        (isCopsPickable ? copsWhoPickAnObject : car.pickedItems).Remove(pickableObject);
    }

    public void RemovePickableObject(int index, bool isCopsPickable = false)
    {
        (isCopsPickable ? copsWhoPickAnObject : car.pickedItems).RemoveAt(index);
    }

    public void RemoveAllPickableObjects(bool isCopsPickable = false)
    {
        (isCopsPickable ? copsWhoPickAnObject : car.pickedItems).Clear();
    }

    public void RemoveAllPickables(bool isCopsPickable = false)
    {
        if (isCopsPickable)
        {
            if (copsWhoPickAnObject.Count > 0)
            {
                RemoveAllPickableObjects(true);
            }
            else
            {
                RemovePickableObject(0, true);
            }
        }
        else
        {
            if (car.pickedItems.Count > 0)
            {
                RemoveAllPickableObjects();
            }
            else
            {
                RemovePickableObject(0);
            }
        }
    }

    public void SetParentTransform(GameObject pickableObject, Transform parentTransform)
    {
        pickableObject.transform.parent = parentTransform;
    }

    public void SetPickableSocketPosition(Transform pickableSocket)
    {
        pickableSocket.position = carPickableSocket.position;
    }

    public void ResetPickableSocketPosition()
    {
        carPickableSocket.position = Vector3.zero;
    }
}