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
            WheelCarController controller = other.GetComponent<WheelCarController>();
            controller.rb.velocity *= controller.speedPercentKeptAtImpact;
            if (controller.rb.velocity.magnitude < controller.damageTakenMaxSpeed)
            {
                controller.lifePoints -= controller.damagesPerAttacks;
                controller.fillImage.fillAmount = (float)controller.lifePoints / controller.maxLifePoints;
                if (controller.lifePoints < 0)
                {
                    SceneManager.LoadScene("PROTO SCENE");
                }
            }
            else
            {
                agent.EnableRagdoll();
                GetComponent<Collider>().enabled = false;
            }
        }
        
        if (other.CompareTag("PlayerBonusCollider"))
        {
            agent.EnableRagdoll();
            GetComponent<Collider>().enabled = false;
        }
    }
}
