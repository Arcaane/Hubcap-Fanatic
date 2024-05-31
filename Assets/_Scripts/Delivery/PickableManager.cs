using System.Collections.Generic;
using HubcapCarBehaviour;
using UnityEngine;
using UnityEngine.Serialization;

public class PickableManager : MonoBehaviour
{
    private static PickableManager _instance;
    public static PickableManager Instance => _instance;

    [Header("Socket to take the object")] public Transform carPickableSocket;
    public Transform worldSocket;

    [Header("Object Possess")] 
    public List<GameObject> copsWhoPickAnObject = new();
    [FormerlySerializedAs("car")] public PlayerCarController playerCar;

/*
    public void Start()
    {
        _instance = this;
        playerCar = PlayerCarController.Instance;
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
        (isCopsPickable ? copsWhoPickAnObject : playerCar.pickedItems).Add(pickableObject);
    }

    public void RemovePickableObject(GameObject pickableObject, bool isCopsPickable = false)
    {
        (isCopsPickable ? copsWhoPickAnObject : playerCar.pickedItems).Remove(pickableObject);
    }

    public void RemovePickableObject(int index, bool isCopsPickable = false)
    {
        (isCopsPickable ? copsWhoPickAnObject : playerCar.pickedItems).RemoveAt(index);
    }

    public void RemoveAllPickableObjects(bool isCopsPickable = false)
    {
        (isCopsPickable ? copsWhoPickAnObject : playerCar.pickedItems).Clear();
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
            if (playerCar.pickedItems.Count > 0)
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
    }*/
}