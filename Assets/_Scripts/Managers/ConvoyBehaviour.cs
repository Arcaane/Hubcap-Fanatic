using System;
using System.Collections;
using System.Collections.Generic;
using Abilities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class ConvoyBehaviour : MonoBehaviour , IDamageable
{
    [SerializeField] private int hp = 500;
    [SerializeField] private int tokenToGiveOnDestroy = 2;
    [SerializeField] private int slotUnlockOnDestroy = 1;
    
    [Header("POLICE CAR")]
    public Vector3 target;
    public int currentTarget = 0;
    public float t;
    public float targetDetectionRange = 3;
    public float speed, normalSpeed,attackModeSpeed;

    public Transform player;
    public NavMeshAgent agent;
    
    [Header("CONVOY")]
    
    
    public float detectionTimer = 3;
    public float detectionDelay = 3;
    public PoliceCarBehavior[] defenseCars;
    public DefensePoint[] defensePoints;
    
    public float attackTimer = 1;
    public float timeBetweenAttack = 1;
    
    
    public float repulsiveRadius;
    public float alignementRadius;
    public float attractiveRadius;
    
    public float playerDetectionRadius;
    public float playerLostRadius;
    
    public float defenseAlignementRadius;
    public float defenseAttractiveRadius;
    
    public bool showRadiusGizmos;

    public bool attackMode;
    public float oneBarZone,twoBarsZone,threeBarZone,fourBarZone;
    
    

    public void Initialize()
    {
        defenseCars = new PoliceCarBehavior[defensePoints.Length];
        
        for (int i = 0; i < defensePoints.Length; i++)
        {
            defenseCars[i] = Instantiate(defensePoints[i].defenseCar, defensePoints[i].point.position,
                Quaternion.identity).GetComponent<PoliceCarBehavior>();
            
            defenseCars[i].target = CarController.instance.transform;
            defenseCars[i].convoyBehaviour = this;
            defenseCars[i].defensePoint = defensePoints[i].point;
        }
        player = CarController.instance.transform;
        agent.SetDestination(target);

    }
    
    private void Update()
    {
        if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(target.x, target.z)) <
            5)
        {
            ConvoyManager.instance.waitingForConvoy = true;
            for (int i = 0; i < defenseCars.Length; i++)
            {
                Destroy(defenseCars[i].gameObject);
            }
            Destroy(gameObject);
        }
        
        
        if (attackMode) AttackMode();
        else
        {
            agent.speed = Mathf.Lerp(agent.speed, normalSpeed, Time.deltaTime * 5);
            
            if (Vector3.SqrMagnitude(transform.position - player.position) < playerDetectionRadius * playerDetectionRadius)
            {
                detectionDelay -= Time.deltaTime;
                if (detectionDelay <= 0)
                {
                    attackMode = true;
                    detectionDelay = detectionTimer;
                }
            }
            else
            {
                detectionDelay = Mathf.Clamp(detectionDelay + Time.deltaTime, 0, detectionTimer);
            }
        }


        SetRadar();
    }


    void SetRadar()
    {
        float dist = Vector3.SqrMagnitude(transform.position - player.position);
        float power;
        if (dist > oneBarZone * oneBarZone)
        {
            power = 0;
        }
        else if (dist > twoBarsZone * twoBarsZone)
        {
            power = 1;
        }
        else if (dist > threeBarZone * threeBarZone)
        {
            power = 2;
        }
        else if (dist > fourBarZone * fourBarZone)
        {
            power = 3;
        }
        else 
        {
            power = 4;
        }

        float dot = Vector2.Dot((new Vector2(transform.position.x,transform.position.z) - new Vector2(player.position.x,player.position.z)).normalized, new Vector2(player.forward.x,player.forward.z).normalized);
        float dotpower;
        if (dot < 0)
        {
            dotpower = 0;
        }
        else if (dot < 0.5f)
        {
            dotpower = 1;
        }
        else if (dot < 0.75f)
        {
            dotpower = 2;
        }
        else if (dot < 0.9f)
        {
            dotpower = 3;
        }
        else
        {
            dotpower = 4;
        }
        
        UIManager.instance.radar.SetActivation(Mathf.RoundToInt(power * 0.6f + dotpower * 0.4f));
    }

    void AttackMode()
    {
        agent.speed = Mathf.Lerp(agent.speed, attackModeSpeed, Time.deltaTime * 5);

        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
        else
        {
            attackTimer = timeBetweenAttack;
            if (defenseCars.Length > 0)
            {
                for (int i = 0; i < defenseCars.Length; i++)
                {
                    if (!defenseCars[i].attackMode)
                    {
                        defenseCars[i].attackMode = true;
                        break;
                    }
                }
            }
        }

        if (defenseCars.Length > 0)
        {
            if (Vector3.SqrMagnitude(transform.position - player.position) > playerLostRadius * playerLostRadius)
            {
                for (int i = 0; i < defenseCars.Length; i++)
                {
                    defenseCars[i].attackMode = false;
                }

                attackMode = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        
        
        if (!showRadiusGizmos) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position,attractiveRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position,alignementRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,repulsiveRadius);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position,playerDetectionRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position,playerLostRadius);
    }

    public void TakeDamage(int damages)
    {
        if (!IsDamageable()) return;
        hp -= damages;
        if (hp < 1) DestroyConvoy();
    }

    public bool IsDamageable() => gameObject.activeSelf;

    public void DestroyConvoy()
    {
        CarExperienceManager.Instance.AddToken(tokenToGiveOnDestroy);
        
        for (int i = 0; i < slotUnlockOnDestroy; i++)
            CarAbilitiesManager.instance.UnlockAbilitySlot();
        
        ConvoyManager.instance.waitingForConvoy = true;
        for (int i = 0; i < defenseCars.Length; i++)
        {
            defenseCars[i].convoyBehaviour = null;
        }
        Destroy(gameObject);
    }
}
[Serializable]
public struct DefensePoint
{
    public GameObject defenseCar;
    public Transform point;
}