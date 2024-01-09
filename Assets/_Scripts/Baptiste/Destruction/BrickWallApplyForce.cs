using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BrickWallApplyForce : MonoBehaviour
{
    [SerializeField] float _radiusForce = 2;
    [SerializeField] float _radiusKinematic = 4;
    [SerializeField] float _force = 2;
    [SerializeField] float _forceToDestroyWall = 10f;
    [SerializeField] UnityEvent _callWhenApplyForce;
    [SerializeField] private LayerMask layerToCollide;
    [SerializeField] private GameObject ps;
    
    //TODO : Refacto this part
    

    private bool hasAppliedForce = false;
    
    private void Update()
    {
        CheckForColliders();
    }

    public void CheckForColliders()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _radiusKinematic, layerToCollide);
        foreach (var collider in colliders)
        {
            Rigidbody rigidbody = collider.GetComponent<Rigidbody>();
            
        }
    }
    
    public void ApplyForce()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _radiusKinematic);
        for (int i = 0; i < colliders.Length; i++)
        {
            Rigidbody rigidbody = colliders[i].GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.isKinematic = false;
                rigidbody.AddExplosionForce(_force, transform.position, _radiusForce);
            }
        }
        if (_callWhenApplyForce != null)
        {
            _callWhenApplyForce.Invoke();
        }
        hasAppliedForce = false;
    }

    public void ApplyForceWithMultiplier(float forceMultiplier)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _radiusKinematic);
        for (int i = 0; i < colliders.Length; i++)
        {
            Rigidbody rigidbody = colliders[i].GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.isKinematic = false;
                rigidbody.AddExplosionForce(_force * forceMultiplier, transform.position, _radiusForce);
            }
        }
        if (_callWhenApplyForce != null)
        {
            _callWhenApplyForce.Invoke();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _radiusForce);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _radiusKinematic);
    }

    
}
