using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraffColider : MonoBehaviour
{
    public IDamageable enemyDamageable;
    public Transform enemyCar;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemyDamageable = other.GetComponent<IDamageable>();
            enemyCar = other.transform;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (enemyCar && other.transform == enemyCar)
        {
            enemyCar = null;
            enemyDamageable = null;
        }
    }
}
