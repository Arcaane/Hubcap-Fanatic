using System;
using System.Collections.Generic;
using UnityEngine;

public class StraffColider : MonoBehaviour
{
    public List<Transform> enemyCar = new List<Transform>();
    public Camera cam;

    private void Start()
    {
        enemyCar = new List<Transform>();
    }

    private void Update()
    {
        for (int i = 0; i < enemyCar.Count; i++)
        {
            if (!enemyCar[i])
            {
                enemyCar.RemoveAt(i);
                break;
            }
            
            if (!enemyCar[i].gameObject.activeSelf)
            {
                RemoveObjectCar(enemyCar[i]);
            }
        }

        if (enemyCar.Count > 0 && CarController.instance.canShoot())
        {
            UIManager.instance.shootIcon.position = cam.WorldToScreenPoint(enemyCar[0].position);
            UIManager.instance.shootIcon.localScale = Vector3.Lerp(UIManager.instance.shootIcon.localScale,Vector3.one, Time.deltaTime*5);
        }
        else
        {
            UIManager.instance.shootIcon.localScale = Vector3.Lerp(UIManager.instance.shootIcon.localScale,Vector3.zero, Time.deltaTime*10);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if(other.GetComponent<ConvoyBehaviour>()) enemyCar.Insert(0,other.transform);
            else enemyCar.Add(other.transform);
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
        }
    }
}
