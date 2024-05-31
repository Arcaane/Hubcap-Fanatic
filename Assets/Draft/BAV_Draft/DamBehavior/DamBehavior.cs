using System;
using System.Collections.Generic;
using HubcapCarBehaviour;
using HubcapManager;
using UnityEngine;

public class DamBehavior : MonoBehaviour
{
    public List<SpawnBlockade> spawnBlockades;
    public List<BasePoliceCarBehavior> policeCarBehaviors;

    public bool isActive;

    [Serializable]
    public struct SpawnBlockade
    {
        public Transform spawnPoint;
        [Pooler] public string enemyKey;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if(!isActive)return;
        
        if (!other.CompareTag("Player")) return;
        Debug.Log("Player pass the dam");
        isActive = false;
        WaveManager.Instance.activeBlockades--;
        foreach (var policeCarBehavior in policeCarBehaviors) policeCarBehavior.EnablePoliceCar();
    }


    public void SpawnCars()
    {
        
        Debug.Log("Spawn Cars");
        for (int i = 0; i < spawnBlockades.Count; i++)
        {
            BasePoliceCarBehavior car = PoolManager.Instance.RetrieveOrCreateObject(spawnBlockades[i].enemyKey, spawnBlockades[i].spawnPoint.position, spawnBlockades[i].spawnPoint.rotation).GetComponent<BasePoliceCarBehavior>();

            if (car != null)
            {
                car.DisablePoliceCar();
                policeCarBehaviors.Add(car);
            }
        }
        
        isActive = true;
    }
}
