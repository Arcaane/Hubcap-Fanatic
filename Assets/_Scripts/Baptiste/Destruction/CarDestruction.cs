using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CarDestruction : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        DestructionManager.instance.ApplyForceCarCollision(other, other.gameObject.GetComponent<Rigidbody>());
        Debug.Log("Collider On Trigger Enter ");
    }
}