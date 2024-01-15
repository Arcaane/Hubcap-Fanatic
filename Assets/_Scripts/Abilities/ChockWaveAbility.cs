using System;
using System.Collections;
using UnityEngine;

public class ChockWaveAbility : MonoBehaviour
{
    [SerializeField] int pointsCount;
    [SerializeField] float maxRadius;
    [SerializeField] float speed;
    [SerializeField] float startWidth;
    [SerializeField] float force;
    [SerializeField] private int abilityDamage = 5;
    [SerializeField] private LayerMask hitByAbility;
    [SerializeField] private float cooldown;
    private bool isBlasting;
    
    private LineRenderer lineRenderer;

    [SerializeField] private Transform car;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = pointsCount + 1;
    }

    private void Start()
    {
        StartCoroutine(Blast());
    }

    private IEnumerator Blast()
    {
        isBlasting = true;
        
        lineRenderer.positionCount = pointsCount + 1;
        float currentRadius = 0f;
        
        Damage(maxRadius);
        
        while (currentRadius < maxRadius)
        {
            currentRadius += Time.deltaTime * speed;
            Draw(currentRadius);
            yield return null;
        }

        yield return new WaitForSeconds(cooldown);
        isBlasting = false;
    }

    private void Damage(float currentRadius)
    {
        Collider[] hittingObjects = Physics.OverlapSphere(car.position, currentRadius, hitByAbility);
        for (int i = 0; i < hittingObjects.Length; i++) hittingObjects[i].GetComponent<IDamageable>()?.TakeDamage(abilityDamage);
        
        // for (int i = 0; 1 < hittingObjects.Length; i++)
        // {
        //     Rigidbody rb = hittingObjects[i].GetComponent<Rigidbody>();
        //     if (!rb) continue;
        //
        //     Vector3 direction = (hittingObjects[i].transform.position - transform.position).normalized;
        //     rb.AddForce(direction * force, ForceMode.Impulse);
        // }
    }

    private void Draw(float currentRadius)
    {
        float angleBetweenPoints = 360f / pointsCount;
        
        for(int i = 0; i <= pointsCount; i++)
        {
            float angle = i * angleBetweenPoints * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0f);
            Vector3 position = direction * currentRadius;

            lineRenderer.SetPosition(i, position);
        }

        lineRenderer.widthMultiplier = Mathf.Lerp(0f, startWidth, 1f - currentRadius / maxRadius);
    }

    private void Update()
    {
        transform.position = car.position;

        if (!isBlasting)
        {
            StartCoroutine(Blast());
        }
    }
}