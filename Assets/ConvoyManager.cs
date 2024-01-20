using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ConvoyManager : MonoBehaviour
{
    public ConvoyBehaviour currentConvoy;
    public static ConvoyManager instance;
    public GameObject[] convoysPrefabs;
    public Rail rails;
    public int convoyPower = 0;




    private void Awake()
    {
        instance = this;
    }
    
    

    public void SpawnConvoy()
    {
        if (currentConvoy != null) return;

        GameObject obj = Instantiate(convoysPrefabs[convoyPower]);
        UIIndic.instance.CreateIndicForConvoy(obj);
        currentConvoy = obj.transform.GetComponent<ConvoyBehaviour>();
        currentConvoy.distancedNodes = rails.distancedNodes;
        currentConvoy.Initialize();
        if (UIIndic.instance.Obj.Count > 3)
        {
            UIIndic.instance.Obj[3] = obj;
        }

        convoyPower++;
    }
}

