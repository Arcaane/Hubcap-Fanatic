using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemolitionBallBehavior : MonoBehaviour
{
    [SerializeField] private float distance = 8;
    [SerializeField] private float upperDistance = 20;
    [SerializeField] private float force;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Rigidbody carRb;
    [SerializeField] private Transform chainPoint;

    private void FixedUpdate()
    {
        float dist = Vector3.Distance(carRb.transform.position, rb.transform.position);
        float overDist = Mathf.Clamp(dist - distance, 0, upperDistance);
        float factor = overDist / upperDistance;
        Vector3 dir = carRb.transform.position - rb.transform.position;
        rb.AddForceAtPosition(dir.normalized * force * factor,chainPoint.position,ForceMode.Force);
    }
}
