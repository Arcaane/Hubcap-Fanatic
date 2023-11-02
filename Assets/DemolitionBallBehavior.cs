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
    [SerializeField] private LineRenderer chain;
    [SerializeField] private bool chained;
    private void FixedUpdate()
    {
        if (!chained) return;
        float dist = Vector3.Distance(carRb.transform.position, rb.transform.position);
        float overDist = Mathf.Clamp(dist - distance, 0, upperDistance);
        float factor = overDist / upperDistance;
        Vector3 dir = carRb.transform.position - rb.transform.position;
        rb.AddForceAtPosition(dir.normalized * force * factor * carRb.velocity.magnitude,chainPoint.position,ForceMode.Force);
        if(dist > upperDistance) rb.MovePosition(carRb.transform.position - dir.normalized * upperDistance);
        Debug.DrawLine(carRb.transform.position,rb.transform.position,Color.Lerp(Color.green, Color.red, factor));
        chain.SetPosition(0,chainPoint.position);
        chain.SetPosition(1,carRb.transform.position);
        transform.localScale = Vector3.Lerp(transform.localScale,Vector3.one *4 ,Time.deltaTime *5);
    }

    public void Setup()
    {
        transform.position = carRb.transform.position - carRb.transform.forward;
        transform.localScale = Vector3.zero;
        chained = true;
        chain.gameObject.SetActive(true);
    }
    
    public void UnChain()
    {
        chained = false;
        chain.gameObject.SetActive(false);
    }
}
