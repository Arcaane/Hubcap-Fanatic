using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraffColider : MonoBehaviour
{
    public PoliceCarBehavior enemyCar;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemyCar = other.GetComponent<PoliceCarBehavior>();   
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == enemyCar.gameObject) enemyCar = null;
    }
}
