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
    
    //Properties that going to be share
    [Header("Car Collider")] 
    //[SerializeField] private LayerMask layerCar;
    [SerializeField] private Rigidbody carRB;
    [SerializeField] private Collider carCollision;
    
    [Header("Brick Parameters")]
    //[SerializeField] private LayerMask layerToCollideWithBrick;
    [SerializeField] private float forceToMoveBrick;
    [SerializeField] private float forceApplyToBrick;

    [Header("Enable Debug Log")] 
    [SerializeField] private bool enabledDebug = true;

    public void ApplyForceCarCollision(Collider elementToCollide, Rigidbody rbObject)
    {
        //if (elementToCollide.gameObject.layer != LayerMask.NameToLayer("DestructionCar")) return;
        if (enabledDebug)
        {
            Debug.Log("Good Layer");
        }
        if (rbObject == null) return;
        if (!(carRB.velocity.magnitude > forceToMoveBrick)) return;
        rbObject.isKinematic = false;
        Vector3 norm = carRB.velocity.normalized * forceApplyToBrick;
        rbObject.AddForce(norm, ForceMode.VelocityChange);
    }
    
    public void GlobalApplyForceOnCollisionEnter(Collision elementToCollide, LayerMask layerToCollide, Rigidbody rb, float forceToMove, float strength)
    {
        if (rb != null && elementToCollide.rigidbody)
        {
            if (elementToCollide.rigidbody.velocity.magnitude > forceToMove)
            {
                rb.isKinematic = false;
                Vector3 norm = elementToCollide.rigidbody.velocity.normalized * strength;
                rb.AddForce(norm, ForceMode.VelocityChange);
            }
        }
    }
    
    /*    
    public LayerMask ReturnBrickLayerToCollide()
    {
        return layerToCollideWithBrick;
    }
    */

    public float ReturnBrickForce()
    {
        return forceToMoveBrick;
    }
    
    public float ReturnForceApplyToBrick()
    {
        return forceApplyToBrick;
    }
    
    
    private bool CompareLayer(int layer, LayerMask layerMask)
    {
        return (layerMask & (1 << layer)) != 0;
    }
}
