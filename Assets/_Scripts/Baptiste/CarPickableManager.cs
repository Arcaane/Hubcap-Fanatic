using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPickableManager : MonoBehaviour
{
    private static CarPickableManager _instance;
    public static CarPickableManager Instance => _instance;

    public Transform _pickableSocket;
    public Transform worldSocket;
    public List<GameObject> _pickableObjects;

    public void Start()
    {
        _instance = this;
    }

    public void AddPickableObject(GameObject pickableObject)
    {
        _pickableObjects.Add(pickableObject);
    }

    public void RemovePickableObject(GameObject pickableObject)
    {
        _pickableObjects.Remove(pickableObject);
    }
    
    public void RemovePickableObject(int index)
    {
        _pickableObjects.RemoveAt(index);
    }
    
    public void RemoveAllPickableObjects()
    {
        _pickableObjects.Clear();
    }
    
    public void RemoveAllPickables()
    {
        if(_pickableObjects.Count > 1)
        {
            RemoveAllPickableObjects();
        }
        else
        {
            RemovePickableObject(0);
        }
    }
    
    public void SetParentTransform(GameObject pickableObject, Transform parentTransform)
    {
        pickableObject.transform.parent = parentTransform;
    }

    public void SetPickableSocketPosition(Transform pickableSocket)
    {
        pickableSocket.position = _pickableSocket.position;
    }
    
    public void ResetPickableSocketPosition()
    {
        _pickableSocket.position = Vector3.zero;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<IPickupable>() != null)
        {
            other.gameObject.GetComponent<IPickupable>().OnPickedUp();
        }
    }
}