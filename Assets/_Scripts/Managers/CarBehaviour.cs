using System;
using Abilities;
using UnityEngine;

public class CarBehaviour : MonoBehaviour
{
    [Header("REFERENCES")] 
    [SerializeField] public Rigidbody rb;
    [SerializeField] public Wheel[] wheels;

    [Header("SUSPENSION")] 
    [SerializeField] private float suspensionLenght = 0.5f;
    [SerializeField] private float suspensionForce = 70f;
    [SerializeField] private float suspensionDampening = 3f;
    [SerializeField] private float anchoring = 0.2f;
    
    [Header("WHEEL")] 
    [SerializeField] private float wheelMass = 0.1f;
    [SerializeField] public float maxSpeed = 25f;
    [SerializeField] public float targetSpeed = 25f;
    [SerializeField] private AnimationCurve accelerationBySpeedFactor;
    [SerializeField] private float acceleration = 12f;
    [SerializeField] private float braking = 12f;
    [SerializeField] private float decceleration = 4f;
    [SerializeField] private float steeringSpeed = 50f;
    
    [Header("PHYSICVALUES")]
    [SerializeField] private Vector3 localCenterOfMass;
    [Tooltip("La rotation Max de la voiture sur les axes X et Z")]
    [SerializeField] private float maxRotation;
    public float directionalDampMultiplier = 1;
    
    [Header("BRAKEDRIFT")] 
    [SerializeField] private float dampeningMultiplier = 0.25f;
    [SerializeField] private float steeringMultiplier = 0.95f;
    [SerializeField] private float accelMultiplier = 1.5f;
    [SerializeField] private float angleMinToExitDrift = 0.1f;
    [SerializeField] protected ParticleSystem[] driftSparks;
    
    public float speedFactor => rb.velocity.magnitude / targetSpeed;
    
    
    private bool driftEngaged;
    private float driftValue;
    public bool brakeMethodApplied;

    // INPUT VALUES
    
    public float accelForce, brakeForce;
    public bool driftBrake;

    public float rotationValue;

    
    private void Start()
    {
        rb.centerOfMass = localCenterOfMass;
    }

    public void OnMove()
    {
        //Debug.Log(rb.velocity.magnitude);
        for (int i = 0; i < wheels.Length; i++)
        {
            if (wheels[i].steeringFactor > 0)
            {
                wheels[i].wheelVisual.localRotation = wheels[i].transform.localRotation = 
                    Quaternion.Lerp(wheels[i].transform.localRotation,Quaternion.Euler(0,rotationValue * wheels[i].steeringFactor * (driftBrake ? steeringMultiplier : 1),0),Time.deltaTime * steeringSpeed);
            }
        }
        
        driftValue = 1- Mathf.Abs(Vector3.Dot(new Vector3(rb.velocity.normalized.x,0,rb.velocity.normalized.z), transform.forward));

        // SORTIE DU DRIFT BRAKE SI ON SE REALIGNE AVEC LA VELOCITE
        if (driftBrake)
        {
            if (!driftEngaged && driftValue > angleMinToExitDrift + 0.1f)
            {
                driftEngaged = true;
            }
            else if (driftEngaged && driftValue < angleMinToExitDrift)
            {
                driftBrake = false;
                driftEngaged = false;
                
                if (driftSparks.Length < 1) return;
                foreach (var t in driftSparks) t.Stop();
                CarAbilitiesManager.instance.DesactivateDriftAbilities();
            }
        }
    }
    
    public void ApplyWheelForces()
    {

        for (int i = 0; i < wheels.Length; i++)
        {
            // APPLICATION DES FORCES
            Vector3 wheelForce = GetWheelForces(wheels[i]);
            rb.AddForceAtPosition(wheelForce,wheels[i].transform.position);
        }
        
    }
    
    public void AddWheelForces(Vector3 force)
    {
        for (int i = 0; i < wheels.Length; i++)
        {
            rb.AddForceAtPosition(force,wheels[i].transform.position);
        }
    }
    
    #region Wheel Methods
    Vector3 GetWheelForces(Wheel wheel)
    {
        Vector3 wheelForce = Vector3.zero;

        // SI ROUE AU SOL, ALORS FORCES
        if (Physics.Raycast(wheel.transform.position, -wheel.transform.up, out RaycastHit hit, suspensionLenght + anchoring))
        {
            // CALCUL DES FORCES
            float suspension = GetWheelSuspensionForce(wheel,hit);
            float directionalDamp = GetWheelDirectionalDampening(wheel);
            float drivingForce = wheel.drivingFactor > 0 ? GetWheelAcceleration(wheel) : 0;
            wheelForce = wheel.transform.up * suspension +
                         wheel.transform.right * directionalDamp +
                         wheel.transform.forward * drivingForce;

                         
            // DEBUG RAYS
            Debug.DrawRay(wheel.transform.position,wheel.transform.right * directionalDamp ,Color.red);
        }

        return wheelForce;
    }
    
    float GetWheelSuspensionForce(Wheel wheel,RaycastHit hit)
    {
        float force = 0;
        
        float offset = suspensionLenght - hit.distance;
        Vector3 wheelWorldVelocity = rb.GetPointVelocity(wheel.transform.position);
        float velocity = Vector3.Dot(wheel.transform.up, wheelWorldVelocity);
        force = (offset * suspensionForce) - (velocity * suspensionDampening);
        wheel.wheelVisual.position = wheel.transform.position - wheel.transform.up * (hit.distance - 0.25f);
        
        return force;
    }
    
    float GetWheelDirectionalDampening(Wheel wheel)
    {
        float force = 0;
        
        Vector3 wheelWorldVelocity = rb.GetPointVelocity(wheel.transform.position);
        float tangentSpeed = Vector3.Dot(wheelWorldVelocity,wheel.transform.right);
        float counterAcceleration = (-tangentSpeed * wheel.directionalDampening) / Time.fixedDeltaTime;
        
        force = wheelMass * counterAcceleration * (driftBrake ? dampeningMultiplier : 1);
        
        return force;
    }
    
    float GetWheelAcceleration(Wheel wheel)
    {
        float force = 0;
        float accel = accelForce * acceleration * accelerationBySpeedFactor.Evaluate(speedFactor) * wheel.drivingFactor * (driftBrake ? accelMultiplier : 1);
        
        float brake = 0;
        if (Vector3.Dot(rb.velocity, transform.forward) > 0.1f)
        {
            brake = brakeForce * -braking * wheel.drivingFactor * (driftBrake ? 0 :1);
            brakeMethodApplied = false;
        }
        else
        {
            if (!brakeMethodApplied)
            {
                brakeMethodApplied = true;
                PlayerBrake();
            }
            
            brake = brakeForce * -decceleration * wheel.drivingFactor;
        }

        force = accel + brake;
        
        return force;
    }
    
    #endregion

    protected virtual void PlayerBrake()
    {
        
    }
}


[Serializable]
public struct Wheel
{
    public Transform transform,wheelVisual;
    public float directionalDampening;
    public float drivingFactor;
    public float steeringFactor;
}