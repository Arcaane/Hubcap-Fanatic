using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructionManager : MonoBehaviour
{
    //Destruction Manager Singleton
    public static DestructionManager instance { get; private set; }
    private void Awake() 
    { 
        if (instance != null && instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            instance = this; 
        } 
    }
    
    //Properties that goind to be share
    [Header("Car Collider")] 
    [SerializeField] private Rigidbody carRB;
    [SerializeField] private Collider carCollider;
    
    [Header("Brick Parameters")]
    [SerializeField] private LayerMask layerToCollideWithBrick;
    [SerializeField] private float forceToMoveBrick;

    public LayerMask ReturnBrickLayerToCollide()
    {
        return layerToCollideWithBrick;
    }
    
    public float ReturnBrickForce()
    {
        return forceToMoveBrick;
    }
}
