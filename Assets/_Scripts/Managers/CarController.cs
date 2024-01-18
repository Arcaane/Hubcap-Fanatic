using System.Collections.Generic;
using Abilities;
using ManagerNameSpace;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using Key = UnityEngine.InputSystem.Key;

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
    [SerializeField] public float nitroSpeed = 50;
    [SerializeField] private ParticleSystem smoke, smokeNitro;
    [SerializeField] public bool nitroMode, canNitro;
    [SerializeField] private float nitroTime;
    [SerializeField] private float nitroDuration;
    [SerializeField] public float nitroRegen;
    
    [Header("JUMP")] 
    [SerializeField] private ParticleSystem jumpSmoke;

    [Header("SHOTGUN")] 
    [SerializeField] private StraffColider straffColider;
    [SerializeField] private float straffTime;
    [SerializeField] public float straffDuration = 2f;
    [SerializeField] private ParticleSystem shotgunParticles;
    [SerializeField] public int shotgunDamages = 50;
    public bool isStraffing;
    public bool isBomber;


    [Header("ROAD DETECTION")] 
    [SerializeField] private LayerMask roadMask;
    [SerializeField] public float offRoadSpeed = 10;
    
    public bool canStraff => straffTime >= straffDuration;

    [HideInInspector] public float dirCam;
    public Transform cameraHolder;
    public float camDist;

    public List<GameObject> pickedItems = new();

    public State currentCarState;
    public bool isDefault => !driftBrake && !isStraffing && !brakeMethodApplied && !nitroMode;
    
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
        base.Update();
        
        rotationValue = stickValue.x;
        
        OnMove();
        
        // SORTIE DU DRIFT BRAKE SI ON LACHE L'ACCELERATION
        if (driftBrake && accelForce < 0.1f)
        {
            CarAbilitiesManager.instance.OnStateExit.Invoke();
            driftBrake = false;
            foreach (var t in driftSparks) t.Stop();
        }
        
        if (nitroMode)
        {
            if (nitroTime > 0)
            {
                nitroTime -= Time.deltaTime;   
                UIManager.instance.SetNitroJauge(nitroTime/nitroDuration);
                CarAbilitiesManager.instance.OnUpdate.Invoke();
            }
            else
            {
                nitroMode = false;
                smoke.Play();
                smokeNitro.Stop();
                targetSpeed = maxSpeed;
                CarAbilitiesManager.instance.OnStateExit.Invoke();
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
            
            if (Physics.Raycast(transform.position + Vector3.up*5, Vector3.down, Mathf.Infinity, roadMask))
            {
                targetSpeed = maxSpeed;
            }
            else
            {
                targetSpeed = offRoadSpeed;
                if (speedFactor > 1)
                {
                    rb.velocity = Vector3.Lerp(rb.velocity,Vector3.ClampMagnitude(rb.velocity, targetSpeed),Time.deltaTime*5); 
                }
            }
            Debug.DrawRay(transform.position + Vector3.up*5,Vector3.down * 1000,Color.blue);
        }

        if (straffTime < straffDuration)
        {
            straffTime += Time.deltaTime;
            //UIManager.instance.SetStraffJauge(straffTime / straffDuration);
        }
    }
    
    void FixedUpdate()
    {
        dirCam = Mathf.Lerp(dirCam, rb.velocity.magnitude,Time.fixedDeltaTime*3);
        ApplyWheelForces();
        // CAMERA
        //cameraHolder.position = Vector3.Lerp(cameraHolder.position,transform.position + rb.velocity.normalized * dirCam * 0.5f,5 * Time.fixedDeltaTime);
        cameraHolder.position = Vector3.Lerp(cameraHolder.position,transform.position + rb.velocity.normalized * dirCam * 0.5f * camDist,5 * Time.fixedDeltaTime);
        
        /*cameraHolder.rotation = Quaternion.Lerp(cameraHolder.rotation,  Quaternion.Euler(0,transform.eulerAngles.y, 0),500 * Time.fixedDeltaTime);
        cameraHolder.position = Vector3.Lerp(cameraHolder.position,transform.position /**+ rb.velocity.normalized * dirCam * 0.5f * camDist,50 * Time.fixedDeltaTime);*/
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
            CarAbilitiesManager.instance.OnStateEnter.Invoke();
            canNitro = false;
            smoke.Stop();
            smokeNitro.Play();
            targetSpeed = nitroSpeed;
        }
        
        if (context.canceled && nitroMode)
        {
            nitroMode = false;
            smoke.Play();
            smokeNitro.Stop();
            targetSpeed = maxSpeed;
            CarAbilitiesManager.instance.OnStateExit.Invoke();
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
        if (context.started && canStraff && !isBomber)
        {
            if (straffColider.enemyCar.Count > 0)
            {
                ShotgunHit();
            }
            
            straffTime = 0;
        }

        if (context.started && isBomber && canStraff)
        {
            Pooler.instance.SpawnInstance(ManagerNameSpace.Key.OBJ_Mine , transform.position, Quaternion.identity);
            straffTime = 0;
        }
    }

    public void ShotgunHit()
    {
        Vector3 direction = straffColider.enemyCar[0].position - transform.position;
        if (Vector3.Dot(direction,transform.right) > 0)
        {
            rb.AddForce(-transform.right * 100);
        }
        else
        {
            rb.AddForce(transform.right * 100);
        }
        shotgunParticles.transform.rotation = Quaternion.LookRotation(direction);
        shotgunParticles.Play();
        straffColider.enemyDamageable[0].TakeDamage(shotgunDamages);
        
        CarAbilitiesManager.instance.OnEnemyDamageTaken.Invoke(straffColider.enemyCar[0].gameObject);
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

            if (other.gameObject.CompareTag("Wall"))
            {
                CarAbilitiesManager.instance.OnWallCollision.Invoke(other);
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
        //CarAbilitiesManager.instance.OnBrake.Invoke();
    }

    public override void OnStopPlayerDrift()
    {
        CarAbilitiesManager.instance.OnStateExit.Invoke();
    }
}

