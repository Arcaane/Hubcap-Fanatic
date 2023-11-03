using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    private LayerMask layerCollide;
    public float _forceToMove = 0.5f;
    public float _strength = 1.2f;

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
        layerCollide = DestructionManager.instance.ReturnBrickLayerToCollide();
    }
    

    public void ResetRB()
    {
        rb.isKinematic = true;
    }

    private void OnCollisionEnter(Collision elementToCollider)
    {
        var hitLayerMask = 1 << elementToCollider.gameObject.layer; 
        Debug.Log( "Hit Layer Mask : " + hitLayerMask);
        Debug.Log( "layerCollide : " + layerCollide.value);
        if ((layerCollide.value & hitLayerMask) != 0)
        {
            Debug.Log("Good Layer");
            if (rb != null && elementToCollider.rigidbody)
            {
                Debug.Log("has RB");
                if (elementToCollider.rigidbody.velocity.magnitude > _forceToMove)
                {
                    Debug.Log("Apply Force");
                    rb.isKinematic = false;
                    Vector3 norm = elementToCollider.rigidbody.velocity.normalized * _strength;
                    rb.AddForce(norm, ForceMode.VelocityChange);
                }
            }
        }
    }
}
