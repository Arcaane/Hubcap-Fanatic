using UnityEngine;
using UnityEngine.InputSystem;

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

    [Header("NITRO")] 
    [SerializeField] private float nitroSpeed = 50;
    [SerializeField] private ParticleSystem smoke, smokeNitro;
    [SerializeField] private bool nitroMode;

    [Header("JUMP")] 
    [SerializeField] private ParticleSystem jumpSmoke;

    [HideInInspector] public float dirCam;
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
        cameraHolder.position = Vector3.Lerp(cameraHolder.position,transform.position + rb.velocity.normalized * dirCam * 0.5f,5 * Time.fixedDeltaTime);
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
			driftBrake = true;
            brakeForce = context.ReadValue<float>();
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
    
    public void AButton(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            nitroMode = true;
            smoke.Stop();
            smokeNitro.Play();
            targetSpeed = nitroSpeed;
        }
        if (context.canceled)
        {
            nitroMode = false;
            smoke.Play();
            smokeNitro.Stop();
            targetSpeed = maxSpeed;
        }
        
    }
    
    public void BButton(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            
            jumpSmoke.Play();
            rb.AddForce(Vector3.up * 300);
        }
        
	}

    #endregion
    
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Enemy"))
        {
            //Debug.Log(other.relativeVelocity.magnitude);
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
                        wheels[i].wheelVisual.localRotation = wheels[i].transform.localRotation = Quaternion.Euler(0, 0, 0);
                    }
                }

                Destroy(Instantiate(fxBounce, other.contacts[0].point, Quaternion.LookRotation(other.contacts[0].normal)),2);
            }
                
            //transform.rotation = Quaternion.Euler(Mathf.Clamp(transform.eulerAngles.x,-maxRotation,maxRotation),transform.eulerAngles.y,Mathf.Clamp(transform.eulerAngles.z,-maxRotation,maxRotation));
        }

        if (other.gameObject.CompareTag("Enemy"))
        {
            if (PickableManager.Instance.carPickableObjects.Count > 0)
            {
                PickableManager.Instance.carPickableObjects[0].gameObject.GetComponent<IPickupable>().OnDrop();
            } 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy")) // EnemyCollision
        {
            if (Vector3.Dot( other.transform.position - transform.position, transform.forward) > 0.75f)
            {
                Debug.Log("Dégats aux enemis");
                other.GetComponent<IDamageable>()?.TakeDamage(1);
            }
        }
    }
}

