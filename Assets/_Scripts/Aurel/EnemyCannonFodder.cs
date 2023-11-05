using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyCannonFodder : MonoBehaviour
{
    private NavMeshAgent agent;
    
    [SerializeField]  public Transform playerPos;
    [SerializeField] public Rigidbody[] ragdollHandler;
    [SerializeField] public List<Collider> ragdollColliders;
    public bool isDead;

    // Start is called before the first frame update
    void Start()
    {
        ragdollHandler = GetComponentsInChildren<Rigidbody>();
        for (int i = 0; i < ragdollHandler.Length; i++)
        {
            ragdollColliders.Add(ragdollHandler[i].GetComponent<Collider>());
        }
        agent = GetComponent<NavMeshAgent>();
    }
    
    private float timer = 0;
    public float updatePath = 0.7f;
    // Update is called once per frame
    void Update()
    {
        if (isDead) return;
        
        timer += Time.deltaTime;
        if (timer > updatePath)
        {
            agent.SetDestination(playerPos.position);
            timer = 0;
        }
    }

    public void EnableRagdoll()
    {
        foreach (var r in ragdollHandler)
        {
            r.isKinematic = false;
        }
        foreach (var r in ragdollColliders)
        {
            r.enabled = true;
        }

        isDead = true;
        agent.enabled = false;
    }   
}
