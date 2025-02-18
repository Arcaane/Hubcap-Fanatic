using System.Collections.Generic;
using Abilities;
using ManagerNameSpace;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : CarBehaviour
{
    public static CarController instance;
    
    [Header("WALLBOUNCE")]
    [Tooltip("Le pourcentage de vitesse gardée lors d'un wallBounce")]
    [SerializeField] public float speedRetained = 0.7f;
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
    [SerializeField] private float[] shootTimes;
    [SerializeField] public float shootDuration;
    [SerializeField] private ParticleSystem shotgunParticles;
    [SerializeField] public int shotgunDamages = 50;
    public bool isStraffing;
    
    public bool isBomber;
    public bool gotVayneUpgrade;
    public int shotBeforeCritAmount;
    public float vaynePassiveMultiplier;
    private int currentShotBeforeCount = 2;

    [Header("ROAD DETECTION")] 
    [SerializeField] private LayerMask roadMask;
    [SerializeField] public float offRoadSpeed = 10;
    [SerializeField] public float offRoadDeccelerationFactor = 5;

    [Header("Effects")] 
	[SerializeField] public GameObject shield;
    public bool isShield;

    public bool isBerserk;
    [HideInInspector] public float dirCam;
    public Transform cameraHolder;
    public float camDist;

    public float pillValue;
    public bool overSpeedPill;

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
        
        currentCollsionBeforeDropDeliver = CollsionBeforeDropDeliver;
        shield.SetActive(false);
        isShield = false;
    }

    public override void Update()
    {
        base.Update();
        
        rotationValue = stickValue.x;
        
        if (CarHealthManager.instance.Lifepoints > 1) {
            OnMove();
        }
        
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
                    rb.velocity = Vector3.Lerp(rb.velocity,Vector3.ClampMagnitude(rb.velocity, targetSpeed),Time.deltaTime*offRoadDeccelerationFactor); 
                }
            }
        }
        
        for (int i = 0; i < shootTimes.Length; i++)
        {
            if (shootTimes[i] < shootDuration)
            {
                shootTimes[i] += Time.deltaTime;
                UIManager.instance.SetShotJauge(shootTimes[i] / shootDuration,i);
            }
        }

        if (isShield)
        {
            shield.transform.position = transform.position;
        }
    }

    public bool canShoot()
    {
        bool result = false;
        for (int i = 0; i < shootTimes.Length; i++)
        {
            if (shootTimes[i] >= shootDuration) result = true;
        }
        return result;
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
        if (context.started)
        {
            if (!(stickValue.x > 0.6f || stickValue.x < -0.6f) && !driftBrake)
            {
                CarAbilitiesManager.instance.OnPill.Invoke();
            }
        }
        
        if (context.performed)
        {
            brakeForce = context.ReadValue<float>();
            if (!driftBrake && (stickValue.x > 0.6f || stickValue.x < -0.6f))
            {
                driftBrake = true;
                CarAbilitiesManager.instance.OnBeginDrift.Invoke();
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
            CarAbilitiesManager.instance.OnBeginNitro.Invoke();
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
        }
    }
    #endregion
    
    public int CollsionBeforeDropDeliver = 3;
    private int currentCollsionBeforeDropDeliver;
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
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

            CarAbilitiesManager.instance.OnWallCollision.Invoke(other);
            
            //transform.rotation = Quaternion.Euler(Mathf.Clamp(transform.eulerAngles.x,-maxRotation,maxRotation),transform.eulerAngles.y,Mathf.Clamp(transform.eulerAngles.z,-maxRotation,maxRotation));
        }
        
        if (other.gameObject.CompareTag("Enemy") && pickedItems.Count > 0)
        {
            currentCollsionBeforeDropDeliver--;

            if (currentCollsionBeforeDropDeliver > 0) return;
            
            for (int i = 0; i < pickedItems.Count; i++)
            {
                ObjectPickable obj = pickedItems[i].GetComponent<ObjectPickable>();
                obj.OnDrop();
                obj.rb.AddForce(other.contacts[0].normal.normalized * 100);
            }
            
            pickedItems.Clear();
            currentCollsionBeforeDropDeliver = CollsionBeforeDropDeliver;
        }
    }
    
    protected override void PlayerBrake()
    {
        base.PlayerBrake();
        CarAbilitiesManager.instance.OnPill.Invoke();
    }

    public override void OnStopPlayerDrift() => CarAbilitiesManager.instance.OnStateExit.Invoke();

    #region PlayerWeapon
    public float mightPowerUpLevel;
    
    public void XButton(InputAction.CallbackContext context)
    {
        if (context.started && canShoot() && !isBomber)
        {
            if (straffColider.enemyCar.Count > 0)
            {
                ShotgunHit();
                UIManager.instance.GoodShotUI(shootTimes[0] >= shootDuration ? 0 : 1);
            }
            else
            {
                CarAbilitiesManager.instance.OnShotgunUsedWithoutTarget.Invoke();
                UIManager.instance.ShootMissUI(shootTimes[0] >= shootDuration ? 0 : 1);
            }

            for (int i = 0; i < shootTimes.Length; i++)
            {
                if (shootTimes[i] >= shootDuration)
                {
                    shootTimes[i] = 0;
                    break;
                }
            }
            
            CarAbilitiesManager.instance.OnShotgunUsed.Invoke();
            CameraShake.instance.SetShake(0.3f);
        }

        if (context.started && isBomber && canShoot())
        {
            Pooler.instance.SpawnInstance(ManagerNameSpace.Key.OBJ_Mine , transform.position, Quaternion.identity);
            
            for (int i = 0; i < shootTimes.Length; i++)
            {
                if (shootTimes[i] >= shootDuration)
                {
                    shootTimes[i] = 0;
                    break;
                }
            }
        }
    }


    private void ShotgunHit()
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

        
        currentShotBeforeCount--;
        if (gotVayneUpgrade && currentShotBeforeCount == 0)
        {
            straffColider.enemyCar[0].GetComponent<IDamageable>()?.TakeDamage(Mathf.FloorToInt((shotgunDamages * mightPowerUpLevel) * vaynePassiveMultiplier));
            currentShotBeforeCount = shotBeforeCritAmount;
        }
        else
        {
            straffColider.enemyCar[0].GetComponent<IDamageable>()?.TakeDamage(Mathf.FloorToInt(shotgunDamages * mightPowerUpLevel));
        }
        
        CarAbilitiesManager.instance.OnEnemyDamageTaken.Invoke(straffColider.enemyCar[0].gameObject);
        CarAbilitiesManager.instance.OnEnemyHitWithShotgun.Invoke(straffColider.enemyCar[0].gameObject);
    }
    #endregion
}

