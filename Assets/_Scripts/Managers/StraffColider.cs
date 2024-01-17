using System;
using System.Collections.Generic;
using UnityEngine;

public class StraffColider : MonoBehaviour
{
    public List<IDamageable> enemyDamageable = new List<IDamageable>();
    public List<Transform> enemyCar = new List<Transform>();

    private void Start()
    {
        enemyCar = new List<Transform>();
        enemyDamageable = new List<IDamageable>();
    }

    private void Update()
    {
        for (int i = 0; i < enemyCar.Count; i++)
        {
            if (!enemyCar[i].gameObject.activeSelf)
            {
                RemoveObjectCar(enemyCar[i]);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemyDamageable.Add(other.GetComponent<IDamageable>());
            enemyCar.Add(other.transform);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        RemoveObjectCar(other.transform);
    }

    public void RemoveObjectCar(Transform car)
    {
        if (enemyCar.Contains(car))
        {
            enemyCar.Remove(car);
            enemyDamageable.Remove(car.GetComponent<IDamageable>());
        }
    }
}
