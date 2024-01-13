using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PickableManager : MonoBehaviour
{
    private static PickableManager _instance;
    public static PickableManager Instance => _instance;

    [Header("Socket to take the object")]
    public Transform carPickableSocket;
    public Transform worldSocket;
    
    [Header("Target Cops Direction")]
    public List<Transform> copsTargetPointIfIsTake;
    [Header("Object Possess")]
    public List<GameObject> carPickableObjects;
    public List<GameObject> copsPickableObjects;
    


    public void Start()
    {
        _instance = this;
    }

    public void AddPickableObject(GameObject pickableObject, bool isCopsPickable = false)
    {
        (isCopsPickable ? copsPickableObjects : carPickableObjects).Add(pickableObject);
    }

    public void RemovePickableObject(GameObject pickableObject, bool isCopsPickable = false)
    {
        (isCopsPickable ? copsPickableObjects : carPickableObjects).Remove(pickableObject);
    }

    public void RemovePickableObject(int index, bool isCopsPickable = false)
    {
        (isCopsPickable ? copsPickableObjects : carPickableObjects).RemoveAt(index);
    }

    public void RemoveAllPickableObjects(bool isCopsPickable = false)
    {
        (isCopsPickable ? copsPickableObjects : carPickableObjects).Clear();
    }
    
    public void RemoveAllPickables(bool isCopsPickable = false)
    {
        if(isCopsPickable)
        {
            if (copsPickableObjects.Count > 0)
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
               if (carPickableObjects.Count > 0)
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