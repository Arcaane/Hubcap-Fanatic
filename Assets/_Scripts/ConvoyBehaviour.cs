using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ConvoyBehaviour : MonoBehaviour
{
    [Header("POLICE CAR")]
    public Vector3 target;
    public int currentTarget = 0;
    public float t;
    public float targetDetectionRange = 3;
    public float speed, normalSpeed,attackModeSpeed;

    public Transform player;
    
    [Header("CONVOY")]
    
    
    public float detectionTimer = 3;
    public float detectionDelay = 3;
    public PoliceCarBehavior[] defenseCars;
    
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
    

    public RadarDetectorUI radar;
    

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        

        transform.parent.position = new Vector3(Random.Range(-730f, 730f), 1.4f, -730);
        
        target = new Vector3(Random.Range(-730f, 730f), 1.4f, 730);
        for (int i = 0; i < defenseCars.Length; i++)
        {
            defenseCars[i].target = CarController.instance.transform;
            
        }
        player = CarController.instance.transform;
        transform.localPosition =Vector3.zero;
        
        
    }
    
    private void Update()
    {
        transform.position += (target - transform.position).normalized * speed * Time.deltaTime;
        transform.forward = (target - transform.position).normalized;

        if (transform.position.z > 725)
        {
            Initialize();
        }
        
        if (attackMode) AttackMode();
        else
        {
            speed = Mathf.Lerp(speed, normalSpeed, Time.deltaTime * 5);
            
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

        float dist = Vector3.SqrMagnitude(transform.position - player.position);
        if (dist > oneBarZone * oneBarZone)
        {
            radar.SetActivation(0);
        }
        else if (dist > twoBarsZone * twoBarsZone)
        {
            radar.SetActivation(1);
        }
        else if (dist > threeBarZone * threeBarZone)
        {
            radar.SetActivation(2);
        }
        else if (dist > fourBarZone * fourBarZone)
        {
            radar.SetActivation(3);
        }
        else 
        {
            radar.SetActivation(4);
        }

    }


    void AttackMode()
    {
        speed = Mathf.Lerp(speed, attackModeSpeed, Time.deltaTime * 5);

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
}
