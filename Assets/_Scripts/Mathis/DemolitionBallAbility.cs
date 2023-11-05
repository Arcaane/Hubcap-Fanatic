using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemolitionBallAbility : CarAbility
{
    [Header("ABILITY PARAMETERS")]
    [SerializeField] private float distance = 8;
    [SerializeField] private float upperDistance = 20;
    [SerializeField] private float force;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Rigidbody carRb;
    [SerializeField] private Transform chainPoint;
    [SerializeField] private float timer = 5;
    [SerializeField] private Transform[] chain;
    private float timerCd = 8;
    
    public override void Activate()
    {
        base.Activate();
        gameObject.SetActive(true);
        transform.position = carRb.transform.position - carRb.transform.forward;
        transform.localScale = Vector3.zero;
        timerCd = timer;
        
    }
    public override void Execute()
    {
        float dist = Vector3.Distance(carRb.transform.position, rb.transform.position);
        float overDist = Mathf.Clamp(dist - distance, 0, upperDistance);
        float factor = overDist / upperDistance;
        Vector3 dir = carRb.transform.position - rb.transform.position;
        rb.AddForceAtPosition(dir.normalized * force * factor * carRb.velocity.magnitude,chainPoint.position,ForceMode.Force);
        if(dist > upperDistance) rb.MovePosition(carRb.transform.position - dir.normalized * upperDistance);
        Debug.DrawLine(carRb.transform.position,rb.transform.position,Color.Lerp(Color.green, Color.red, factor));
        transform.localScale = Vector3.Lerp(transform.localScale,Vector3.one *4 ,Time.deltaTime *5);
        for (int i = 0; i < chain.Length; i++)
        {
            chain[i].position = Vector3.Lerp(chainPoint.position,carRb.transform.position,(1/((float)chain.Length+1))*(i+1));
        }
        
        if (timerCd > 0)
        {
            timerCd -= Time.deltaTime;
        }
        else
        {
            activated = false;
            gameObject.SetActive(false);
        }
    }
}
