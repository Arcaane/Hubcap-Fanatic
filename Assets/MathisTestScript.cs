using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathisTestScript : MonoBehaviour
{
    public Material mat;
    public Transform car;

    private void Update()
    {
        mat.SetVector("_CarPos",new Vector4(car.position.x,car.position.z,0,0));
    }
}
