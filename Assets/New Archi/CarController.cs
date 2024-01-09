using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CarController : MonoBehaviour
{
    #region Variables
    [Header("REFERENCES")] 
    [SerializeField] public Rigidbody rb;
    [SerializeField] public Wheel[] wheels;
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private Material bodyMat;

    [Header("SUSPENSION")] 
    [SerializeField] private float suspensionLenght = 0.5f;
    [SerializeField] private float suspensionForce = 70f;
    [SerializeField] private float suspensionDampening = 3f;
    [SerializeField] private float anchoring = 0.2f;
    
    [Header("WHEEL")] 
    [SerializeField] private float wheelMass = 0.1f;
    [SerializeField] private float maxSpeed = 25f;
    [SerializeField] private AnimationCurve accelerationBySpeedFactor;
    [SerializeField] private float acceleration = 12f;
    [SerializeField] private float braking = 12f;
    [SerializeField] private float decceleration = 4f;
    [SerializeField] private AnimationCurve steeringByTriggerFactor;
    [SerializeField] private float steeringSpeed = 50f;

    [Header("PHYSICVALUES")]
    [SerializeField] private Vector3 localCenterOfMass;
    
    [Header("BRAKEDRIFT")] 
    [SerializeField] private float dampeningMultiplier = 0.25f;
    [SerializeField] private float steeringMultiplier = 0.95f;
    [SerializeField] private float accelMultiplier = 1.5f;
    [SerializeField] private float angleMinToExitDrift = 0.1f;
    
    [Header("WALLBOUNCE")]
    [Tooltip("Le pourcentage de vitesse gardée lors d'un wallBounce")]
    [SerializeField] private float speedRetained = 0.7f;
    [Tooltip("Le pourcentage de vitesse Max gardée lors d'un wallBounce")]
    [SerializeField] private float maxSpeedRetained = 0.8f;
    [Tooltip("L'angle Minimum ( 1 = 90° ) pour WallBounce")]
    [SerializeField] private float minAngleToBounce = 0.3f;
    [SerializeField] private GameObject fxBounce;

    private bool driftEngaged;
    private float driftValue;
    private float dirCam;

    // INPUT VALUES
    private Vector2 stickValue;
    private float accelForce, brakeForce;
    private bool driftBrake;
    
    #endregion
    
    private void Start()
    {
        rb.centerOfMass = localCenterOfMass;
    }
    
    private void Update()
    {
        //Debug.Log(rb.velocity.magnitude);
        for (int i = 0; i < wheels.Length; i++)
        {
            if (wheels[i].steeringFactor > 0)
            {
                wheels[i].wheelVisual.localRotation = wheels[i].transform.localRotation = Quaternion.Lerp(wheels[i].transform.localRotation,Quaternion.Euler(0,stickValue.x * wheels[i].steeringFactor * steeringByTriggerFactor.Evaluate(accelForce) * (driftBrake ? steeringMultiplier : 1),0),Time.deltaTime * steeringSpeed);
            }
        }
        
        driftValue = 1- Mathf.Abs(Vector3.Dot(new Vector3(rb.velocity.normalized.x,0,rb.velocity.normalized.z), transform.forward));

        // SORTIE DU DRIFT BRAKE SI ON LACHE L'ACCELERATION
        if (driftBrake && accelForce < 0.1f)
        {
            driftBrake = false;
            bodyMat.color = new Color(1,0.6f,0);
        }

        // SORTIE DU DRIFT BRAKE SI ON SE REALIGNE AVEC LA VELOCITE
        if (driftBrake )
        {
            if (!driftEngaged && driftValue > angleMinToExitDrift + 0.1f)
            {
                driftEngaged = true;
            }
            else if (driftEngaged && driftValue < angleMinToExitDrift)
            {
                driftBrake = false;
                driftEngaged = false;
                bodyMat.color = new Color(1,0.6f,0);
            }
        }
    }
    
    void FixedUpdate()
    {
        
        dirCam = Mathf.Lerp(dirCam, rb.velocity.magnitude,Time.fixedDeltaTime*3);
        
        
        for (int i = 0; i < wheels.Length; i++)
        {
            // APPLICATION DES FORCES
            Vector3 wheelForce = GetWheelForces(wheels[i]);
            rb.AddForceAtPosition(wheelForce,wheels[i].transform.position);
        }
        // CAMERA
        cameraHolder.position = Vector3.Lerp(cameraHolder.position,transform.position + rb.velocity.normalized * dirCam * 0.5f,5*Time.fixedDeltaTime);
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
                         wheel.transform.forward * drivingForce ;
            
            // DEBUG RAYS
            Debug.DrawRay(wheel.transform.position,wheel.transform.up * suspension,Color.green);
            Debug.DrawRay(wheel.transform.position,wheel.transform.right * directionalDamp,Color.red);
            Debug.DrawRay(wheel.transform.position,wheel.transform.forward * drivingForce,Color.blue);
            Debug.DrawRay(wheel.transform.position,wheelForce,Color.white);
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

        float factor = rb.velocity.magnitude / maxSpeed;
        
        float accel = accelForce * acceleration * accelerationBySpeedFactor.Evaluate(factor) * wheel.drivingFactor * (driftBrake ? accelMultiplier : 1);

        float brake = 0;
        if (Vector3.Dot(rb.velocity, transform.forward) > 0.1f)
        {
            brake = brakeForce * -braking * wheel.drivingFactor * (driftBrake ? 0 :1);
        }
        else
        {
            brake = brakeForce * -decceleration * wheel.drivingFactor;
        }

        force = accel + brake;
        
        return force;
    }
    
    #endregion

    #region Inputs
    public void RShoulder(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            accelForce = context.ReadValue<float>();
        }
        else
        {
            accelForce = 0;
        }
    }
    
    public void LShoulder(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            brakeForce = context.ReadValue<float>();
            if (stickValue.x > 0.6f || stickValue.x < -0.6f)
            {
                driftBrake = true;
                bodyMat.color = Color.cyan;
            }
        }
        else
        {
            brakeForce = 0;
        }
    }

    public void LStick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            stickValue = context.ReadValue<Vector2>();
        }
        else
        {
            stickValue = Vector2.zero;
        }
    }
    #endregion
    
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            Debug.Log(other.relativeVelocity.magnitude);
            if (Vector3.Dot(other.contacts[0].normal, transform.forward) < -minAngleToBounce)
            {
                    
                Vector2 reflect = Vector2.Reflect(new Vector2(transform.forward.x, transform.forward.z),
                    new Vector2(other.contacts[0].normal.x,other.contacts[0].normal.z));
                transform.forward = new Vector3(reflect.x,0, reflect.y);
                rb.velocity = transform.forward * other.relativeVelocity.magnitude * speedRetained;
                rb.angularVelocity = Vector3.zero;
                    
                for (int i = 0; i < wheels.Length; i++)
                {
                    if (wheels[i].steeringFactor > 0)
                    {
                        wheels[i].wheelVisual.localRotation =
                            wheels[i].transform.localRotation = Quaternion.Euler(0, 0, 0);
                    }
                }

                Destroy(Instantiate(fxBounce, other.contacts[0].point, Quaternion.LookRotation(other.contacts[0].normal)),2);
            }
                
            //transform.rotation = Quaternion.Euler(Mathf.Clamp(transform.eulerAngles.x,-maxRotation,maxRotation),transform.eulerAngles.y,Mathf.Clamp(transform.eulerAngles.z,-maxRotation,maxRotation));
        }
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
