using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MathisTestScript : MonoBehaviour
{
    public Material mat;
    public Transform car;
    public TMP_Text text;
    public float timer;

    private void Update()
    {
        timer += Time.deltaTime;
        text.text = Mathf.Floor(timer / 60) + ":" + Mathf.Floor(timer % 60);
        mat.SetVector("_CarPos",new Vector4(car.position.x,car.position.z,0,0));
    }
}
