using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemolitionBallAbility : Ability
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
    
    public override void StartAbility()
    {
        base.StartAbility();
        gameObject.SetActive(true);
        transform.position = carRb.transform.position - carRb.transform.forward;
        transform.localScale = Vector3.zero;
        timerCd = timer;
        
    }
    
}
