using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class WheelCarController : MonoBehaviour
{
    #region Variables
    [Header("REFERENCES")] 
    [SerializeField] public Rigidbody rb;
    [SerializeField] public Wheel[] wheels;
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private Material bodyMat;

    [Header("SUSPENSION")] 
    [SerializeField] private float suspensionLenght = 0.3f;
    [SerializeField] private float suspensionForce = 35f;
    [SerializeField] private float suspensionDampening = 2f;
    [SerializeField] private float anchoring = 0.2f;
    
    [Header("WHEEL")] 
    [SerializeField] private float wheelMass = 0.1f;
    [SerializeField] private float maxSpeed = 15f;
    [SerializeField] private AnimationCurve accelerationBySpeedFactor;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float braking = 5f;
    [SerializeField] private float decceleration = 5f;
    [SerializeField] private AnimationCurve steeringByTriggerFactor;
    [SerializeField] private float steeringSpeed = 5f;

    [Header("PHYSICVALUES")]
    [SerializeField] private Vector3 localCenterOfMass;
    
    [Header("BRAKEDRIFT")] 
    [SerializeField] private float dampeningMultiplier = 0.25f;
    [SerializeField] private float steeringMultiplier = 0.25f;
    [SerializeField] private float accelMultiplier = 0.25f;
    [SerializeField] private float angleMinToExitDrift = 0.1f;

    private bool driftEngaged;
    private float driftValue;
    private float dirCam;

    [Header("KIT")] 
    [SerializeField] private CarKitManager kitManager;

    // INPUT VALUES
    private Vector2 stickValue;
    private float accelForce, brakeForce;
    private bool driftBrake;

    private CarDash carDash;
    #endregion
    
    private void Start()
    {
        rb.centerOfMass = localCenterOfMass;
        bodyMat.color = new Color(1,0.6f,0);
        carDash = GetComponent<CarDash>();
    }
    
    private void Update()
    {
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
        if (carDash.IsDashing)
        {
            rb.velocity = carDash.dashForward * carDash.dashSpeed;
            dirCam = Mathf.Lerp(dirCam, maxSpeed,Time.fixedDeltaTime*3);
        }
        else
        {
            dirCam = Mathf.Lerp(dirCam, rb.velocity.magnitude,Time.fixedDeltaTime*3);
        }
        
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
                         wheel.transform.right * (!carDash.IsDashing ? directionalDamp: 0) +
                         wheel.transform.forward * (!carDash.IsDashing ? drivingForce : 0);
            
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
    
    #region Dash
    
    /*
    void Dash()
    {
        isDashing = true;
        dashTimer = dashDuration;
        dashForward = transform.forward;
        rb.angularVelocity = new Vector3(0, 0, 0);
        bodyMat.color = new Color(1,0,0);
        gameObject.layer = 6;
    }
    */
    
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
    
    public void XButton(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        kitManager.ActivateXAbility();
    }
    
    public void YButton(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        kitManager.ActivateNitro();
    }
    
    public void BButton(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        kitManager.ActivateBAbility();
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
}

[Serializable]
public struct Wheel
{
    public Transform transform,wheelVisual;
    public float directionalDampening;
    public float drivingFactor;
    public float steeringFactor;
}

[Serializable]
public struct Event
{
    public int scoreAmount;
    public string text;
}
