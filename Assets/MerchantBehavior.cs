using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MerchantBehavior : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform[] points;
    public float speed;
    public int index;

    public static MerchantBehavior instance;

    private void Awake()
    {
        instance = this;
    }


    void Update()
    {
        if (Vector2.SqrMagnitude(new Vector2(transform.position.x, transform.position.z) -
                                 new Vector2(points[(index + 1) % points.Length].position.x,
                                     points[(index + 1) % points.Length].position.z)) < 25)
        {
            index = (index + 1) % points.Length;
            agent.SetDestination(points[(index + 1) % points.Length].position);
        }
    }

    private void Start()
    {
        agent.SetDestination(points[(index + 1) % points.Length].position);
    }
}
