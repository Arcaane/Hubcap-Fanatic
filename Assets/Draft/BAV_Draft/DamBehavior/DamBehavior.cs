using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamBehavior : MonoBehaviour
{
    public List<PoliceCarBehavior> policeCarBehaviors;
    [SerializeField] private Collider boxCollider;

    private void Start()
    {
        if (boxCollider == null)
        {
            SetupBoxCollider();
        }
    }
    
    void SetupBoxCollider()
    {
        boxCollider = GetComponent<Collider>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        Debug.Log("Player pass the dam");
        foreach (var policeCarBehavior in policeCarBehaviors)
        {
            //Enable PoliceCarEngine
        }
    }
    [ContextMenu("Setup Police Car/Add Police Car Behaviors")]
    void AddPoliceCarBehaviors()
    {
        policeCarBehaviors.Clear();
        PoliceCarBehavior[] childPoliceCarBehaviors = GetComponentsInChildren<PoliceCarBehavior>();
        policeCarBehaviors.AddRange(childPoliceCarBehaviors);
    }
}
