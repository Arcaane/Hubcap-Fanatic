using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            // TODO : C'EST DE LA MERDE, C'EST TEMPORAIRE
            CarController controller = other.GetComponent<CarController>();
           
        }
        
        if (other.CompareTag("PlayerBonusCollider"))
        {
            agent.EnableRagdoll();
            GetComponent<Collider>().enabled = false;
        }
    }
}
