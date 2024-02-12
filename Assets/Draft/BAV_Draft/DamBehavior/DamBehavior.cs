using System;
using System.Collections;
using System.Collections.Generic;
using ManagerNameSpace;
using UnityEngine;

public class DamBehavior : MonoBehaviour
{
    public List<SpawnBlockade> spawnBlockades;
    public List<PoliceCarBehavior> policeCarBehaviors;

    public bool isActive;

    [Serializable]
    public struct SpawnBlockade
    {
        public Transform spawnPoint;
        public Key enemy;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if(!isActive)return;
        
        if (!other.CompareTag("Player")) return;
        Debug.Log("Player pass the dam");
        isActive = false;
        WaveManager.instance.activeBlockades--;
        foreach (var policeCarBehavior in policeCarBehaviors)
        {
            policeCarBehavior.isActive = true;
            
        }
    }


    public void SpawnCars()
    {
        
        Debug.Log("Spawn Cars");
        for (int i = 0; i < spawnBlockades.Count; i++)
        {
            PoliceCarBehavior car = ((Transform)Pooler.instance.SpawnInstance(spawnBlockades[i].enemy, spawnBlockades[i].spawnPoint.position, spawnBlockades[i].spawnPoint.rotation)).GetComponent<PoliceCarBehavior>();

            if (car != null)
            {
                car.isActive = false;
                policeCarBehaviors.Add(car);
            }
        }
        
        isActive = true;
    }
}
