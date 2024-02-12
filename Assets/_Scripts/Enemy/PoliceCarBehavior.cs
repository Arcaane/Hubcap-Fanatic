using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abilities;
using ManagerNameSpace;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class PoliceCarBehavior : CarBehaviour, IDamageable
{
    [Space(4)] [Header("!MISC STATS!")] [SerializeField]
    private Key enemyKey;

    [SerializeField] private int hp = 100;
    private int currentHp;
    [SerializeField] private int carDamage;
    private int currentDamages;
    [SerializeField] private float shootCooldown;
    [SerializeField] private AnimationCurve hpToAddPerWave;
    [SerializeField] private AnimationCurve damageToAddPerWave;
    [SerializeField] private AnimationCurve speedToAddPerWave;
    [SerializeField] private AnimationCurve expToGiveBasedOnLevel;
    [Space(4)] public static List<PoliceCarBehavior> policeCars = new List<PoliceCarBehavior>();

    [Header("POLICE CAR")] 
    public bool isActive = true;
    public Transform target;

    [SerializeField] private Material[] mat;

    public Vector2 randomOffset;
    public Vector2 maxRngOffset;
    public Transform currentTarget;
    public float angleToCollisionDamage = 0.90f;

    [Header("WALLBOUNCE")] [Tooltip("Le pourcentage de vitesse gardée lors d'un wallBounce")] [SerializeField]
    private float speedRetained = 0.7f;

    [Tooltip("Le pourcentage de vitesse Max gardée lors d'un wallBounce")] [SerializeField]
    private float maxSpeedRetained = 0.8f;

    [Tooltip("L'angle Minimum ( 1 = 90° ) pour WallBounce")] [SerializeField]
    private float minAngleToBounce = 0.3f;

    [SerializeField] private GameObject fxBounce;
    [SerializeField] private ParticleSystem fxFire;

    [Header("CONVOY")] public ConvoyBehaviour convoyBehaviour;
    public bool attackMode;
    public Transform defensePoint;

    public float repulsiveRadius;
    public float alignementRadius;
    public float attractiveRadius;

    public bool showRadiusGizmos;

    public bool driveByCar;
    public ParticleSystem shootFx;
    public bool shooting;

    [Header("Pickable")] 
    public GameObject socketPickableCop;
    public GameObject objectPickable;

    public GameObjectDelgate OnPoliceCarDie = delegate {  };
    public MeshRenderer meshR;
    public MeshRenderer[] metalParts;
    public Material metal;
    public bool canDestroyPickable;

    [Header("BERSERK CAR")] 
    public bool runToPlayer;
    public float overshootValue = 12;
    public float randomOvershoot = 2;
    public float driveByPhaseOvershoot = 8;
    public float distanceToPlayer;
    private float saveOvershoot;

    void Start()
    {
        if (policeCars == null) policeCars = new List<PoliceCarBehavior>();
        target = CarController.instance.transform;
        currentTarget = target;
        policeCars.Add(this);
        randomOffset = new Vector2(Random.Range(-maxRngOffset.x, maxRngOffset.x),
            Random.Range(-maxRngOffset.y, maxRngOffset.y));

        mat = new Material[meshR.materials.Length];

        if (Random.value > 0.5f) distanceToPlayer *= -1;
        
        for (int i = 0; i < mat.Length; i++)
        {
            mat[i] = new Material(meshR.materials[i]);
        }
        meshR.materials = mat;
        metal = new Material(metal);
        for (int i = 0; i < metalParts.Length; i++)
        {
            metalParts[i].material = metal;
        }

        saveOvershoot = overshootValue;
    }

    private void OnEnable()
    {
        float currentWave = WaveManager.instance.currentWaveCount;
        
        currentHp = hp + Mathf.FloorToInt(hpToAddPerWave.Evaluate(currentWave));
        currentDamages = carDamage + Mathf.FloorToInt(damageToAddPerWave.Evaluate(currentWave));
        maxSpeed += Mathf.FloorToInt(speedToAddPerWave.Evaluate(currentWave));
        
        fxFire.Stop();
    }

    private void OnDisable()
    {
        overshootValue = saveOvershoot;
        policeCars.Remove(this);
    }
    
    private void Update()
    {
        if (!isActive) return;
        
        base.Update();
        ActiveFireFB();

        if (convoyBehaviour) ConvoyUpdate();
        else if (driveByCar) DriveByUpdate();
        else SoloUpdate();
    }

    private void SoloUpdate()
    {
        if (runToPlayer)
        {
            if (target == null)
            {
                target = CarController.instance.transform;
            }

            Vector3 targetPos = currentTarget.position + currentTarget.forward * overshootValue * CarController.instance.globalSpeedFactor;

            float angleToTarget = Vector2.SignedAngle(new Vector2(transform.forward.x, transform.forward.z),
                new Vector2(targetPos.x, targetPos.z) -
                new Vector2(transform.position.x, transform.position.z));

            rotationValue = -Mathf.Clamp(angleToTarget / 10, -1, 1);

            if (angleToTarget > 90 || angleToTarget < -90) driftBrake = true;
            
            Debug.DrawLine(transform.position,targetPos,Color.cyan);
            
            if (Vector3.Dot((currentTarget.position - transform.position).normalized, -currentTarget.forward) < 0)
            {
                runToPlayer = false;
                overshootValue = saveOvershoot;
                distanceToPlayer *= -1;
            }
        }
        else
        {
            float rot = 0;
            float result;
            int nb = 2;

            Vector3 targetPos = currentTarget.position + currentTarget.right * distanceToPlayer + 
                                currentTarget.forward * driveByPhaseOvershoot;
            float angleToTarget = Vector2.SignedAngle(new Vector2(transform.forward.x, transform.forward.z),
                new Vector2(targetPos.x, targetPos.z) -
                new Vector2(transform.position.x, transform.position.z));
            rot = -Mathf.Clamp(angleToTarget / 10, -1, 1) * 2;

            if (angleToTarget > 90 || angleToTarget < -90) driftBrake = true;

            Debug.DrawLine(transform.position,targetPos,Color.magenta);
            
            if (Vector3.Dot((targetPos - transform.position).normalized, currentTarget.forward) < 0)
            {
                runToPlayer = true;
                RandomOvershoot();
            }
            
            for (int i = 0; i < policeCars.Count; i++)
            {
                if (policeCars[i] == this || !policeCars[i].gameObject.activeSelf) continue;
                result = GetRotationValueToObject(policeCars[i].transform, attractiveRadius, alignementRadius,
                    repulsiveRadius);
                if (result > -10)
                {
                    rot += result;
                    nb++;
                }
            }
            
            rotationValue = rot / nb;
        }
        

        OnMove();
    }

    private float shootingTimer;
    private void DriveByUpdate()
    {
        float rot = 0;
        float result;
        int nb = 2;


        rot += (GetRotationValueToObject(currentTarget, attractiveRadius, alignementRadius, repulsiveRadius, true) +
                GetRotationValueToObject(currentTarget, attractiveRadius, alignementRadius, 0, true));

        for (int i = 0; i < policeCars.Count; i++)
        {
            if (policeCars[i] == this || !policeCars[i].gameObject.activeSelf) continue;
            result = GetRotationValueToObject(policeCars[i].transform, attractiveRadius, alignementRadius,
                repulsiveRadius);
            if (result > -10)
            {
                rot += result;
                nb++;
            }
        }

        Vector3 direction = currentTarget.position - transform.position;
        float sqrDist = direction.sqrMagnitude;
        if (sqrDist < attractiveRadius * attractiveRadius)
        {
            float dot = Vector2.Dot(
                new Vector2(currentTarget.forward.x, currentTarget.forward.z),
                new Vector2(transform.position.x, transform.position.z) -
                new Vector2(currentTarget.position.x, currentTarget.position.z));

            float value = (Mathf.Clamp((dot * -1) / 2.5f, -1, 1) + 1) / 2;
            float speedvalue = Mathf.Lerp(0, maxSpeed, value);
            targetSpeed = Mathf.Lerp(targetSpeed, speedvalue, Time.deltaTime * 5);

            //Debug.Log("Shoot : " + dot);

            if (!shooting && dot > -2f && dot < 1f)
            {
                shootFx.transform.rotation = Quaternion.LookRotation(direction);
                shooting = true;
                
                shootFx.Play();
            }

            if (shooting && !(dot > -2f && dot < 1f))
            {
                shooting = false;
                shootFx.Stop();
            }

            if (shooting)
                shootFx.transform.rotation = Quaternion.Lerp(shootFx.transform.rotation,
                    Quaternion.LookRotation(direction), Time.deltaTime * 5);

            if (shooting)
            {
                shootingTimer += Time.deltaTime;
                if (shootingTimer > shootCooldown)
                {
                    CarHealthManager.instance.TakeDamage(currentDamages);
                    shootingTimer = 0f;
                }
            }
        }
        else
        {
            targetSpeed = Mathf.Lerp(targetSpeed, maxSpeed, Time.deltaTime * 5);
            if (shooting)
            {
                shooting = false;
                shootFx.Stop();
            }
        }

        rotationValue = rot / nb;

        OnMove();
    }

    private void ConvoyUpdate()
    {
        if (!attackMode) BoidUpdate();
        else if (driveByCar) DriveByUpdate();
        else SoloUpdate();
    }


    private void BoidUpdate()
    {
        float rot = 0;
        float result;
        int nb = 2;

        rot += GetRotationValueToObject(convoyBehaviour.transform, convoyBehaviour.attractiveRadius,
            convoyBehaviour.alignementRadius, convoyBehaviour.repulsiveRadius, true, true);
        rot += GetRotationValueToObject(defensePoint, convoyBehaviour.defenseAttractiveRadius,
            convoyBehaviour.defenseAlignementRadius, 0, true, true) * 2;

        Vector3 direction = defensePoint.position - transform.position;
        float sqrDist = direction.sqrMagnitude;
        if (sqrDist < attractiveRadius * 2 * attractiveRadius * 2)
        {
            float dot = Vector2.Dot(
                new Vector2(convoyBehaviour.transform.forward.x, convoyBehaviour.transform.forward.z),
                new Vector2(transform.position.x, transform.position.z) -
                new Vector2(defensePoint.position.x, defensePoint.position.z));

            float value = (Mathf.Clamp((dot * -1) / 2.5f, -1, 1) + 1) / 2;
            float speedvalue = Mathf.Lerp(0, maxSpeed, value);
            targetSpeed = Mathf.Lerp(targetSpeed, speedvalue, Time.deltaTime * 5);
        }
        else
        {
            targetSpeed = Mathf.Lerp(targetSpeed, maxSpeed, Time.deltaTime * 5);
        }


        for (int i = 0; i < convoyBehaviour.defenseCars.Length; i++)
        {
            if (convoyBehaviour.defenseCars[i] == this) continue;
            result = GetRotationValueToObject(convoyBehaviour.defenseCars[i].transform, attractiveRadius,
                alignementRadius, repulsiveRadius);
            if (result > -10)
            {
                rot += result;
                nb++;
            }
        }

        rotationValue = rot / nb;

        OnMove();
    }

    private float GetRotationValueToObject(Transform obj, float attrRad, float alignRad, float repulRad,
        bool alwaysAttract = false, bool applyOffset = false)
    {
        Vector3 direction = obj.position +
                            (applyOffset ? obj.right * randomOffset.x + obj.forward * randomOffset.y : Vector3.zero) -
                            transform.position;
        float sqrDist = direction.sqrMagnitude;
        Vector2 targetDir;

        if (sqrDist > attrRad * attrRad) // En dehors des radius
        {
            //Debug.DrawLine(transform.position,obj.position,Color.magenta);
            if (alwaysAttract)
            {
                targetDir = new Vector2(direction.x, direction.z).normalized;
            }
            else
            {
                return -100;
            }
        }
        else if (sqrDist > alignRad * alignRad) // Radius attractif
        {
            targetDir = new Vector2(direction.x, direction.z).normalized;
            //Debug.DrawLine(transform.position,obj.position,Color.green);
        }
        else if (sqrDist > repulRad * repulRad) // Radius alignement
        {
            targetDir = new Vector2(obj.forward.x, obj.forward.z).normalized;
            //Debug.DrawLine(transform.position,obj.position,Color.yellow);
        }
        else // Radius repulsif
        {
            targetDir = new Vector2(-direction.x, -direction.z).normalized;
            //Debug.DrawLine(transform.position,obj.position,Color.red);
        }

        return GetRotationValueToAlign(targetDir);
    }

    private float GetRotationValueToAlign(Vector2 targetDir)
    {
        float angleToTarget = Vector2.SignedAngle(new Vector2(transform.forward.x, transform.forward.z), targetDir);
        return -Mathf.Clamp(angleToTarget / 10, -1, 1);
    }

    void FixedUpdate()
    {
        if (!isActive) return;
        ApplyWheelForces();
    }

    public void SwapTarget(Transform newTarget, bool isCarHasPick = false)
    {
        currentTarget = isCarHasPick ? newTarget : target;
    }


    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Enemy") ||
            other.gameObject.CompareTag("Player"))
        {
            if (!other.gameObject.CompareTag("Wall"))
            {
                DropItem(other);
            }
            
            if (other.transform.CompareTag("Player"))
            {
                var a = other.transform.transform.position - transform.position;
                a.Normalize();
                var b = transform.forward;
                b.Normalize();

                if (Vector3.Dot(a, b) > angleToCollisionDamage) // EnemyCollision
                {
                    CarHealthManager.instance.TakeDamage(currentDamages);
                }
            }

            if (Vector3.Dot(other.contacts[0].normal, transform.forward) < -minAngleToBounce)
            {
                Vector2 reflect = Vector2.Reflect(new Vector2(transform.forward.x, transform.forward.z),
                    new Vector2(other.contacts[0].normal.x, other.contacts[0].normal.z));
                transform.forward = new Vector3(reflect.x, 0, reflect.y);
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

                Destroy(
                    Instantiate(fxBounce, other.contacts[0].point, Quaternion.LookRotation(other.contacts[0].normal)),
                    2);
            }
            //transform.rotation = Quaternion.Euler(Mathf.Clamp(transform.eulerAngles.x,-maxRotation,maxRotation),transform.eulerAngles.y,Mathf.Clamp(transform.eulerAngles.z,-maxRotation,maxRotation));
        }
    }
    
    private bool isDead = false;
    public void TakeDamage(int damages)
    {
        currentHp -= damages;
        ActiveDamageFB();
        TextEffect txt = Pooler.instance.SpawnTemporaryInstance(Key.OBJ_TextEffect, transform.position + Vector3.up * 5,
            quaternion.identity, 0.7f).GetComponent<TextEffect>();
        txt.SetDamageText(damages);
        txt.transform.parent = CarController.instance.cameraHolder;

        if (currentHp > 1 && !isDead) return;
        Die();
    }

    private void Die()
    {
        isDead = true;
        
        if (objectPickable != null)
        {
            objectPickable.GetComponent<ObjectPickable>().OnDrop();
            objectPickable.GetComponent<ObjectPickable>().rb.AddForce(Vector3.up * 100);
            objectPickable = null;
        }
        
        // Give gold
        int gGive = Random.Range(1, 4);
        if (isAimEffect) gGive += 10;
        CarAbilitiesManager.instance.GoldAmountWonOnRun += gGive;
        TextEffect txt = Pooler.instance.SpawnTemporaryInstance(Key.OBJ_GoldText, transform.position + Vector3.up * 5,
            quaternion.identity, 1).GetComponent<TextEffect>();
        txt.SetGoldText(gGive);
        txt.transform.parent = CarController.instance.cameraHolder;
        
        CarAbilitiesManager.instance.OnEnemyKilled.Invoke(gameObject);
        CarExperienceManager.Instance.GetExp(Mathf.RoundToInt(expToGiveBasedOnLevel.Evaluate(CarExperienceManager.Instance.playerLevel)));
        
        OnPoliceCarDie.Invoke(gameObject);
        OnPoliceCarDie = null;
        Pooler.instance.DestroyInstance(enemyKey, transform);
    }

    public bool IsDamageable() => gameObject.activeSelf && currentHp > 0;

    private void DropItem(Collision collision)
    {
        if (objectPickable != null)
        {
            objectPickable.GetComponent<ObjectPickable>().OnDrop();
            objectPickable.GetComponent<ObjectPickable>().rb.AddForce(collision.contacts[0].normal.normalized * 100);
            objectPickable = null;
        }
    }

    private void RandomOvershoot()
    {
        float rand = Random.Range(-randomOvershoot, randomOvershoot);
        overshootValue += rand;
    }
    
   
    private async void ActiveDamageFB()
    {
        foreach (var t in mat)
        {
            t.SetFloat("_UseDamage", 1);
        }
        metal.SetFloat("_UseDamage", 1);

        await Task.Delay(300);

        foreach (var t in mat)
        {
            t.SetFloat("_UseDamage", 0);
        }
        metal.SetFloat("_UseDamage", 0);
    }
    
    private void ActiveFireFB()
    {
        if (isScorch)
        {
            fxFire.gameObject.SetActive(true);
            fxFire.Play(true);
        }
        else
        {
            fxFire.gameObject.SetActive(false);
            fxFire.Stop(true);
        }
    }
}