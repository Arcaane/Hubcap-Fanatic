using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FierySmoke : MonoBehaviour
{
    private ParticleSystem particleSystem;
    private float timer;
    [SerializeField] private float durationBetweenSmokeParticles;

    private void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer = durationBetweenSmokeParticles;
            particleSystem.TriggerSubEmitter(0);
        }
    }
}
