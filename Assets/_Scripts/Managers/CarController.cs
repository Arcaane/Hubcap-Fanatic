using System.Collections.Generic;
using Abilities;
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
    [SerializeField] public bool nitroMode, canNitro;
    [SerializeField] private float nitroTime;
    [SerializeField] private float nitroDuration;
    [SerializeField] private float nitroRegen;
    
    [Header("JUMP")] 
    [SerializeField] private ParticleSystem jumpSmoke;

    [Header("STRAFF")] 
    [SerializeField] private StraffColider straffColider;
    [SerializeField] private float straffTime;
    [SerializeField] private float straffDuration;
    [SerializeField] private Animation animation;

    [HideInInspector] public float dirCam;
    public Transform cameraHolder;
    public float camDist;

    public List<GameObject> pickedItems = new();
    
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
            foreach (var t in driftSparks) t.Stop();
            CarAbilitiesManager.Instance.DesactivateDriftAbilities();
        }

        if (nitroMode)
        {
            if (nitroTime > 0)
            {
                nitroTime -= Time.deltaTime;   
                UIManager.instance.SetNitroJauge(nitroTime/nitroDuration);
            }
            else
            {
                nitroMode = false;
                smoke.Play();
                smokeNitro.Stop();
                targetSpeed = maxSpeed;
                CarAbilitiesManager.Instance.DesactivateNitroAbilities();
            }
        }
        else /*if(!canNitro)*/
        {
            if (nitroTime < nitroDuration)
            {
                nitroTime += Time.deltaTime * nitroRegen;   
                UIManager.instance.SetNitroJauge(nitroTime/nitroDuration);
            }
            
            if(nitroTime >= nitroDuration / 4f)
            {
                canNitro = true;
                //nitroTime = nitroDuration;
                //UIManager.instance.SetNitroJauge(1);
            }
        }

        if (straffTime < straffDuration)
        {
            straffTime += Time.deltaTime;
            UIManager.instance.SetStraffJauge(straffTime / straffDuration);
        }
    }
    
    void FixedUpdate()
    {
        dirCam = Mathf.Lerp(dirCam, rb.velocity.magnitude,Time.fixedDeltaTime*3);
        ApplyWheelForces();
        // CAMERA
        //cameraHolder.position = Vector3.Lerp(cameraHolder.position,transform.position + rb.velocity.normalized * dirCam * 0.5f,5 * Time.fixedDeltaTime);
        cameraHolder.position = Vector3.Lerp(cameraHolder.position,transform.position + rb.velocity.normalized * dirCam * 0.5f * camDist,5 * Time.fixedDeltaTime);
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
                CarAbilitiesManager.Instance.ActivateDriftAbilities();
                foreach (var t in driftSparks) t.Play();
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
    
    public void AButton(InputAction.CallbackContext context)
    {
        if (context.started && canNitro)
        {
            nitroMode = true;
            canNitro = false;
            smoke.Stop();
            smokeNitro.Play();
            targetSpeed = nitroSpeed;
            CarAbilitiesManager.Instance.ActivateNitroAbilities();
        }
        
        if (context.canceled && nitroMode)
        {
            nitroMode = false;
            smoke.Play();
            smokeNitro.Stop();
            targetSpeed = maxSpeed;
            CarAbilitiesManager.Instance.DesactivateNitroAbilities();
        }
        
    }
    
    // POUR PLAYTEST
    public void YButton(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (Time.timeScale > 0.5f) Time.timeScale = 0;
            else Time.timeScale = 1;
        }
    }
    
    public void XButton(InputAction.CallbackContext context)
    {
        if (context.started && straffTime >= straffDuration)
        {
            if (straffColider.enemyCar != null)
            {
                if (Vector3.Dot((straffColider.enemyCar.transform.position - transform.position).normalized, transform.right) > 0)
                {
                    animation.Play("StraffLeft");
                }
                else
                {
                    animation.Play("StraffRight");
                }

            }
            straffTime = 0;
        }
    }

    public void StraffHit()
    {
        if (straffColider.enemyCar != null)
        {
            
            straffColider.enemyDamageable.TakeDamage(100);
            Debug.Log("STRAFFED");
                
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

        
        if (other.gameObject.CompareTag("Enemy") && pickedItems.Count > 0)
        {
            for (int i = 0; i < pickedItems.Count; i++)
            {
                pickedItems[i].GetComponent<ObjectPickable>().OnDrop();
            }
            
            pickedItems.Clear();
            //PickableManager.Instance.RemoveAllPickables();
        }
    }
    
    protected override void PlayerBrake()
    {
        base.PlayerBrake();
        CarAbilitiesManager.Instance.OnBrake.Invoke();
    }
}

