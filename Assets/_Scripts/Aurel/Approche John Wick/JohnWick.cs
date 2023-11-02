using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JohnWick : MonoBehaviour
{
    [Header("Targets groups")] 
    Target[] tempTargs;
    public List<Target> targetsInLevel; // Contain all targets that can be reached on load level
    public List<Target> targetsReachable; // Contain all targets that can be reached at this moment

    [Header("Detection Settings")]
    public float detectionAngle; // 
    public float detectionDst; // 

    //public bool canAttack;
    //public bool isAttacking;
    
    void Start()
    {
        tempTargs = FindObjectsOfType<Target>();
        foreach (var t in tempTargs)
        {
            targetsInLevel.Add(t);
        }
    }
    
    void Update()
    {
        foreach (var t in targetsInLevel)
        {
            if (targetsReachable.Contains(t))
            {
                // Si ça fait plus de X secondes, alors peux l'enlever du pool

                if (!IsTargetReachable(t)) 
                    targetsReachable.Remove(t);
            }
            else
            {
                if (IsTargetReachable(t)) 
                    targetsReachable.Add(t);
            }
        }
    }

    private bool IsTargetReachable(Target target)
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward).normalized;
        Vector3 toOther = (target.transform.position - transform.position).normalized;

        bool isAlling = Vector3.Dot(forward, toOther) > (1 - (detectionAngle / 180)); // Calcul le dot product
        
        bool isAtDist = Vector3.Distance(target.transform.position, transform.position) < detectionDst; // Distance

        return isAlling && isAtDist;
    }

    private void OnDrawGizmos()
    {
        foreach (var t in targetsInLevel)
        {
            Gizmos.color = targetsReachable.Contains(t) ? Color.green : Color.red;
            Gizmos.DrawLine(transform.position, t.transform.position);
        }
        
        Vector3 dir = transform.forward;
        dir.y = 0f;
        
        float minusAngle =-detectionAngle / 2f;
        float maxAngle = detectionAngle / 2f;
        var minusDir = Quaternion.Euler(0f, minusAngle, 0f) * dir;
        var maxDir = Quaternion.Euler(0f, maxAngle, 0f) * dir;
        
        Debug.DrawRay(transform.position, minusDir * detectionDst, Color.blue);
        Debug.DrawRay(transform.position, maxDir * detectionDst, Color.blue);
        Debug.DrawLine(transform.position + (minusDir * detectionDst), transform.position + maxDir * detectionDst, Color.blue);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionDst);
    }
}
