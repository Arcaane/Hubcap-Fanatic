using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DestructionBox : MonoBehaviour
{
    [Header("Renderer")] 
    [SerializeField] private GameObject box;
    [SerializeField] private GameObject fracturedBox;
    
    [Header("Explosion")]
    [SerializeField] private float explosionForce = 100;
    [SerializeField] private float explosionRadius = 10;
    
    
    [Header("Debug")]
    public Collider[] colliders;
    public Rigidbody[] rigidbodies;
    private BoxCollider boxCollider;
    
    private Transform player;
    private bool hasExploded = false;
    
    public void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        if (PlayerCarController.Instance != null)
        {
            player = PlayerCarController.Instance.transform;
        }
        else
        {
            Debug.LogError("CarController.instance is not set correctly!");
        }
        EnableCollider();
        EnableKinematics();
        fracturedBox.SetActive(false);
    }

    void EnableCollider()
    {
        if (colliders == null) AddCollidersToArray();
        foreach (var collider in colliders)
        {
            collider.enabled = true;
        }
    }

    void EnableKinematics(bool value = true)
    {
        foreach (var rb in rigidbodies)
        {
            rb.isKinematic = value;
        }
    }
    
    [ContextMenu("Setup/Setup Box")]
    void SetupBox()
    {
        AddRBToArray();
    } 
    
    
    [ContextMenu("Setup/Clear")]
    void ClearArray()
    { 
        rigidbodies = null;
    }
    
    void AddCollidersToArray()
    {
        colliders = GetComponentsInChildren<Collider>();
    } 
    
    void AddRBToArray()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>();
    }
    
    public void DestroyBox()
    {
        ReplaceWithFracturedBox();
        EnableKinematics(false);
        foreach (var rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = false;
            rigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius);
        }
        DestroyObjectAsync();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasExploded)
        {
            hasExploded = true;
            DestroyBox();
        }
    }
    
    async void DestroyObjectAsync()
    {
        await Task.Delay(5000); 
        if (hasExploded)
        {
            Destroy(gameObject);
        }
    }


    void ReplaceWithFracturedBox()
    {
        if (fracturedBox != null)
        {
            fracturedBox.SetActive(true);
            box.SetActive(false);
            boxCollider.enabled = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
