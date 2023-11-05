using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class HonkAbility : CarAbility
{
    [Header("ABILITY PARAMETERS")] 
    [SerializeField] private float radius = 20;
    [SerializeField] private float angle = 60;
    [SerializeField] private float force = 60;
    [SerializeField] private Material mat;
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private Transform carTransform;
    private float visibility;
    
    
    public override void Activate()
    {
        base.Activate();
        mat.SetFloat("_Angle", angle);
        mat.SetFloat("_Visibility", 1);
        visibility = 1;
        gameObject.SetActive(true);
        transform.position = carTransform.position;
        transform.rotation = Quaternion.Euler(0,carTransform.eulerAngles.y,0);
        Collider[] colliders = Physics.OverlapSphere(transform.position,radius);
        Vector3 dir = default;
        for (int i = 0; i < colliders.Length; i++)
        {
            Debug.Log("HONKED AT " + colliders[i].gameObject.name + " / ANGLE : " + Vector3.Angle(dir, transform.forward) + " / TAG : " + colliders[i].tag);
            dir = (colliders[i].transform.position - transform.position).normalized;
            if (colliders[i].CompareTag("Cone") && colliders[i].attachedRigidbody != null && Vector3.Angle(dir, transform.forward) <= angle)
            {
                Debug.Log("PROPULSED " + colliders[i].gameObject.name);
                colliders[i].attachedRigidbody.AddForce(dir * force,ForceMode.VelocityChange);
            }
        }
    }
    
    public override void Execute()
    {
        if (visibility > 0)
        {
            visibility -= Time.deltaTime*2;
            transform.position = carTransform.position;
            transform.rotation = Quaternion.Euler(0,carTransform.eulerAngles.y,0);
        }
        else
        {
            activated = false;
            gameObject.SetActive(false);
        }
        mat.SetFloat("_Visibility", curve.Evaluate(visibility));
        
    }
}
