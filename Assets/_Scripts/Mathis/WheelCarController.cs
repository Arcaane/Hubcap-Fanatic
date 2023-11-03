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
    
    private float dashTimer = 0;
    private float dashCd = 0;
    [HideInInspector] public bool isDashing;
    private Vector3 dashForward;
    [Header("DASH")]
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown;
    [SerializeField] private float dashSpeed;
    
    [Header("SCORING")]
    [SerializeField] public float multiplier;
    [SerializeField] private int score;
    private float scoreDisplayed;
    [SerializeField] private TMP_Text scoreText,multiplierText,eventText,eventScoreText;
    [SerializeField] private Transform scoreTransform;
    private bool eventApplying;
    [SerializeField] private Animation animEvent;
    [SerializeField] private List<Event> events = new List<Event>(0);
    
    public bool onGround;
    public bool onAir;
    private float conePercuteTime;
    private float eventTimer;

    public bool driftEngaged;
    public float driftValue;
    public int driftCursor;
    public float dirCam;


    [Header("PHYSICVALUES")]
    
    // INPUT VALUES
    private Vector2 stickValue;
    private float accelForce, brakeForce;
    private bool driftBrake;
    
    #endregion
    
    private void Start()
    {
        rb.centerOfMass = localCenterOfMass;
        bodyMat.color = new Color(1,0.6f,0);
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

        if (isDashing)
        {
            if (dashTimer > 0) dashTimer -= Time.deltaTime;
            else
            {
                dashCd = dashCooldown;
                isDashing = false;
                bodyMat.color = new Color(1,0.6f,0);
                rb.velocity = dashForward * maxSpeed * 0.8f;
                gameObject.layer = 0;
            }
        }

        if (dashCd > 0) dashCd -= Time.deltaTime;
        else
        {
            dashCd = 0;
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

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + transform.forward * dashSpeed, 3);
    }

    void FixedUpdate()
    {

        if (isDashing)
        {
            rb.velocity = dashForward * dashSpeed;
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
            if (!onGround) onGround = true;
            // CALCUL DES FORCES
            float suspension = GetWheelSuspensionForce(wheel,hit);
            float directionalDamp = GetWheelDirectionalDampening(wheel);
            float drivingForce = wheel.drivingFactor > 0 ? GetWheelAcceleration(wheel) : 0;
            wheelForce = wheel.transform.up * suspension +
                         wheel.transform.right * (!isDashing ? directionalDamp: 0) +
                         wheel.transform.forward * (!isDashing ? drivingForce : 0);
            
            // DEBUG RAYS
            Debug.DrawRay(wheel.transform.position,wheel.transform.up * suspension,Color.green);
            Debug.DrawRay(wheel.transform.position,wheel.transform.right * directionalDamp,Color.red);
            Debug.DrawRay(wheel.transform.position,wheel.transform.forward * drivingForce,Color.blue);
            Debug.DrawRay(wheel.transform.position,wheelForce,Color.white);
        }
        else
        {
            if (onGround)
            {
                onGround = false;
            }
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
    
    public async void Dash(ITargetable target = null)
    {
        isDashing = true;

        Vector3 initPos = default;
        Vector3 finalPos = default;
        
        if (target != null)
        {
            Vector3 forward = transform.TransformDirection(Vector3.forward).normalized;
            Vector3 toOther = (target.Position - transform.position).normalized;
            var align = Vector3.Dot(forward.normalized, toOther.normalized);
            var offset = target.Position + new Vector3(1,0, 1) * align;
        
            initPos = transform.position;
            finalPos = initPos + offset;
        }
        else
        {
            initPos = transform.position;
            finalPos = initPos + transform.forward * dashSpeed;
        }
        
        Quaternion initRotation = transform.rotation;
        Quaternion finalRotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        
        float timer = 0;
        while (timer < dashDuration)
        {
            transform.position = Vector3.Lerp(initPos, finalPos, timer / dashDuration);
            transform.rotation = Quaternion.Lerp(initRotation, finalRotation, timer / dashDuration);
            await Task.Yield();
            timer += Time.deltaTime;
        }
        
        transform.position = finalPos;
        transform.rotation = finalRotation;
        rb.angularVelocity = new Vector3(0, 0, 0);
        isDashing = false;
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
    
    public void XButton(InputAction.CallbackContext context)
    {
        /*if (context.started)
        {
            driftBrake = true;
        } 
        else if (context.canceled)
        {
            driftBrake = false;
        }*/
    }
    
    public void AButton(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (dashCd <= 0 && !isDashing) Dash();
        }
    }

    public void YButton(InputAction.CallbackContext ctx)
    {
        
    }
    
    public void BButton(InputAction.CallbackContext ctx)
    {
        
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
        if (other.transform.CompareTag("Cone") && conePercuteTime <= 0)
        {
            Event newEvent = new Event();
            newEvent.text = "Percutted Cone";
            newEvent.scoreAmount = 150;
            AddEvent(newEvent);
            conePercuteTime = 0.2f;
        }

        if (other.transform.CompareTag("Wall") )
        {
            multiplier = 1;
            score = 0;
            scoreDisplayed = 0;
            multiplierText.text = "x" + multiplier;
            events.Clear();
        }
    }
    
    public void AddEvent(Event newEvent)
    {
        events.Add(newEvent);
    }

    public async void ApplyEvent()
    {
        eventTimer = 8;
        Event appliedEvent = events[0];
        animEvent.Play("EventAnim");
        eventApplying = true;
        score += Mathf.CeilToInt(appliedEvent.scoreAmount * multiplier);
        eventScoreText.text = appliedEvent.scoreAmount.ToString();
        eventText.text = appliedEvent.text;
        multiplier += 0.1f;
        multiplierText.text = "x" + multiplier;
        events.RemoveAt(0);
        await Task.Delay(2000);
        eventApplying = false;
        
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

[Serializable]
public struct Event
{
    public int scoreAmount;
    public string text;
}
