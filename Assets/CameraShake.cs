using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;
    public float shakeTime,shakeForce;
    public Vector3 pos;

    private void Awake()
    {
        instance = this;
        pos = transform.localPosition;
    }

    public void SetShake(float time)
    {
        shakeTime = Mathf.Clamp(time, shakeTime,Mathf.Infinity);
    }

    private void Update()
    {
        transform.localPosition = pos + (transform.up * (Mathf.Sin(Time.time * 50 + 1) * shakeForce) + transform.right * (Mathf.Sin(Time.time * 50) * shakeForce * 2)) * shakeTime;
        if (shakeTime < 0) shakeTime = 0;
        else shakeTime -= Time.deltaTime;
    }
}
