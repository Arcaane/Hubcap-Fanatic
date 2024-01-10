using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using EnemyNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CarController : CarBehaviour
{
    public static CarController instance;
    
    [Header("WALLBOUNCE")]
    [Tooltip("Le pourcentage de vitesse gardée lors d'un wallBounce")]
    [SerializeField] private float speedRetained = 0.7f;
    [Tooltip("Le pourcentage de vitesse Max gardée lors d'un wallBounce")]
    [SerializeField] private float maxSpeedRetained = 0.8f;
    [Tooltip("L'angle Minimum ( 1 = 90° ) pour WallBounce")]
    [SerializeField] private float minAngleToBounce = 0.3f;
    [SerializeField] private GameObject fxBounce;

    
    private float dirCam;
    public Transform cameraHolder;
    

    // INPUT VALUES
    private Vector2 stickValue;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }


    private void Update()
    {

        rotationValue = stickValue.x;
        
        OnMove();

        // SORTIE DU DRIFT BRAKE SI ON LACHE L'ACCELERATION
        if (driftBrake && accelForce < 0.1f)
        {
            driftBrake = false;
            
        }

        
    }
    
    void FixedUpdate()
    {
        
        dirCam = Mathf.Lerp(dirCam, rb.velocity.magnitude,Time.fixedDeltaTime*3);
        
        ApplyWheelForces();
        
        // CAMERA
        cameraHolder.position = Vector3.Lerp(cameraHolder.position,transform.position + rb.velocity.normalized * dirCam * 0.5f,5*Time.fixedDeltaTime);
    }

    

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

        if (other.gameObject.CompareTag("Cone"))
        {
            Destroy(other.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fodder"))
        {
            other.GetComponent<EnemyFoddler>().TakeDamage(10);
        }
    }
}

