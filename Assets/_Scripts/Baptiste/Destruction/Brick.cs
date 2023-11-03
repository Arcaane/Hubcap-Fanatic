using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    private LayerMask layerCollide;
    public float _forceToMove = 0.5f;
    public float _forceApplyToBrick = 1.2f;

    //Private Value
    private Rigidbody rb;
    private bool isKinematic = false;

    private void Awake()
    {
        SetupBrick();
    }

    private void SetupBrick()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        _forceToMove = DestructionManager.instance.ReturnBrickForce();
        _forceApplyToBrick = DestructionManager.instance.ReturnForceApplyToBrick();
        //layerCollide = DestructionManager.instance.ReturnBrickLayerToCollide();
    }
    

    public void ResetRB()
    {
        rb.isKinematic = true;
    }

    private void OnCollisionEnter(Collision elementToCollide)
    {
        if (rb != null && elementToCollide.rigidbody)
        {
            Debug.Log("Has Rigidbody");
            Debug.Log("Has Rigidbody");
            if (elementToCollide.rigidbody.velocity.magnitude > _forceToMove)
            {
                Debug.Log("Has Rigidbody");
                rb.isKinematic = false;
                Vector3 norm = elementToCollide.rigidbody.velocity.normalized * _forceApplyToBrick;
                rb.AddForce(norm, ForceMode.VelocityChange);
            }
        }
        //DestructionManager.instance.ApplyForceCarCollision(elementToCollide, rb);
    }
}
