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

    public Transform player;

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

    [Header("FollowPath")]
    public int previous,next;
    public float speed,targetSpeed,index;
    public List<Vector3> distancedNodes;
    public float completion;
    public float upVector = 1.2f;

    public bool isDead;

    public void Initialize()
    {
        previous = Random.Range(0, distancedNodes.Count);
        next = (previous + 1) % distancedNodes.Count;
        transform.position = Vector3.Lerp(distancedNodes[previous], distancedNodes[next], index)+Vector3.up*upVector;
        transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(distancedNodes[next] - distancedNodes[previous]),Time.deltaTime*5);

        
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
    
    }
    
    void FixedUpdate()
    {
        completion += Time.fixedDeltaTime * speed;
        if (completion >= 1)
        {
            completion--;
        }

        speed = Mathf.Lerp(speed, targetSpeed, Time.fixedDeltaTime * 2);
        
        index = Mathf.Lerp(0, distancedNodes.Count, completion);
        previous = Mathf.FloorToInt(index);
        next = Mathf.CeilToInt(index)%distancedNodes.Count;
        index -= previous;
        
        transform.position = Vector3.Lerp(distancedNodes[previous], distancedNodes[next], index)+Vector3.up*upVector;
        transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(distancedNodes[next] - distancedNodes[previous]),Time.deltaTime*5);
    }
    
    private void Update()
    {
        if (attackMode) AttackMode();
        else
        {
            
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
    }


    

    void AttackMode()
    {

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
        if(isDead) return;
        if (!IsDamageable()) return;
        hp -= damages;
        if (hp < 1) DestroyConvoy();
    }

    public bool IsDamageable() => gameObject.activeSelf;

    public void DestroyConvoy()
    {
        isDead = true;
        for (int i = 0; i < slotUnlockOnDestroy; i++)
            CarAbilitiesManager.instance.UnlockAbilitySlot();
        
        for (int i = 0; i < defenseCars.Length; i++)
        {
            defenseCars[i].convoyBehaviour = null;
        }

        Destroy(gameObject);
        if (tokenToGiveOnDestroy > 0)
        {
            CarExperienceManager.Instance.AddToken(tokenToGiveOnDestroy);
            CarExperienceManager.Instance.shop.StartShopUI();   
        }
    }
}
[Serializable]
public struct DefensePoint
{
    public GameObject defenseCar;
    public Transform point;
}