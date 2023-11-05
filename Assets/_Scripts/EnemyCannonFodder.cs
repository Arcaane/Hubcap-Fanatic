using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyCannonFodder : MonoBehaviour
{
    private NavMeshAgent agent;
    
    [HideInInspector] public Transform playerPos;
    [SerializeField] private Rigidbody[] ragdollHandler;

    // Start is called before the first frame update
    void Start()
    {
        ragdollHandler = GetComponentsInChildren<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
    }
    
    private float timer = 0;
    public float updatePath = 0.45f;
    private bool isDead;
    // Update is called once per frame
    void Update()
    {
        if(isDead) return;
        
        timer += Time.deltaTime;
        if (timer > updatePath)
        {
            var tempVec3 = new Vector3(playerPos.position.x, 0, playerPos.position.z);
            agent.SetDestination(tempVec3);
            timer = 0;
        }
    }

    public void EnableRagdoll()
    {
        foreach (var r in ragdollHandler)
        {
            r.isKinematic = false;
        }
        
        // TODO - Son de mort
        // TODO - Add Score
        isDead = true;
        agent.enabled = false;
    }   
}
