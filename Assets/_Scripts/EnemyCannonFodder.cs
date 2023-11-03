using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyCannonFodder : MonoBehaviour
{
    private NavMeshAgent agent;
    
    [SerializeField]  public Transform playerPos;
    [SerializeField] private Rigidbody[] ragdollHandler;

    // Start is called before the first frame update
    void Start()
    {
        ragdollHandler = GetComponentsInChildren<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
    }
    
    private float timer = 0;
    public float updatePath = 0.7f;
    // Update is called once per frame
    void Update()
    {
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

        agent.enabled = false;
    }   
}
