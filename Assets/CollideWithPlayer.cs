using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollideWithPlayer : MonoBehaviour
{
    private EnemyCannonFodder agent;

    private void Start()
    {
        agent = GetComponent<EnemyCannonFodder>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            agent.EnableRagdoll();
        }
    }
}
