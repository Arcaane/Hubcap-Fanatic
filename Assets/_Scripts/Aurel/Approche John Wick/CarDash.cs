using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;


public class CarDash : MonoBehaviour
{
    [Header("DASH")]
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] public float dashSpeed;
    [SerializeField] private float dashCooldown = 1;
    
    private float dashTimer = 0;
    public float resetSpeed = 10;
    public bool IsDashing => isDashing;
    private bool isDashing;
    
    [HideInInspector] public Vector3 dashForward;
    
    [Header("Targets groups")] 
    private List<ITargetable> targetsInLevel = new(); // Contain all targets that can be reached on load level
    private List<ITargetable> targetsReachable = new(); // Contain all targets that can be reached at this moment

    [Header("Detection Settings")]
    public float detectionAngle;
    public float detectionDst;
    public bool isLeftTrigger = false;
    
    
    void Start()
    {
        var tempTargs = FindObjectsOfType<MonoBehaviour>().ToArray();
        foreach (var t in tempTargs.Where(args => args is ITargetable).Cast<ITargetable>())
        {
            targetsInLevel.Add(t);
        }
        Debug.Log(targetsInLevel.Count);
    }
    
    void Update()
    {
        if (dashCooldown > 0) dashCooldown -= Time.deltaTime;
        else
        {
            dashCooldown = 0;
        }
        
        if(targetsInLevel.Count == 0) return;
        foreach (var t in targetsInLevel)
        {
            if (targetsReachable.Contains(t))
            {
                // Si ça fait plus de X secondes, alors peux l'enlever du pool

                if (!IsTargetReachable(t))
                {
                    targetsReachable.Remove(t);
                }
            }
            else
            {
                if (IsTargetReachable(t)) 
                    targetsReachable.Add(t);
            }
        }
    }

    #region DashGestion
    private void PreDash()
    {
        if (isLeftTrigger)
        {
            Dash(ClosestTarget());
        }
        else
        {
            Dash();
        }
    }

    private async void Dash(ITargetable target = null)
    {
        isDashing = true;

        dashForward = transform.forward;
        // POS
        Vector3 initPos = transform.position;
        Vector3 finalPos = default;
        // ROT
        Quaternion initRotation = transform.rotation;
        Quaternion finalRotation = default;
        
        Vector3 dir = default;
        Vector3 inverseDir = default;
        
        if (target != null) // AS UNE TARGET
        {
            inverseDir = target.Position - transform.position;
            inverseDir.Normalize();
            finalPos = target.Position + (inverseDir * 2);
            finalRotation = Quaternion.LookRotation(inverseDir);
            
        }
        else // DASH NORMAL
        {
            finalPos = initPos + transform.forward * dashSpeed;
            finalPos.y = initPos.y;
            initRotation = transform.rotation;
            finalRotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        }
        
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
        GetComponent<Rigidbody>().velocity = (target == null ? transform.forward : inverseDir) * resetSpeed;
        isDashing = false;
    }
    
    public void AButton(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            if (dashCooldown <= 0 && !isDashing) PreDash();
        }
    }

    public void LBButton(InputAction.CallbackContext ctx)
    {
        isLeftTrigger = ctx.performed;
    }
    #endregion
    
    #region Target Detection
    private bool IsTargetReachable(ITargetable t)
    {
        //Vector3 forward = transform.TransformDirection(Vector3.forward).normalized;
        //Vector3 toOther = (t.Position - transform.position).normalized;
        //bool isAlling = Vector3.Dot(forward, toOther) > (1 - (detectionAngle / 180)); // Calcul le dot product
        
        return Vector3.Distance(t.Position, transform.position) < detectionDst;
    }
    
    public ITargetable ClosestTarget()
    {
        var tempDst = 1000;
        ITargetable returnTargetable = null;
        
        foreach (var t in targetsReachable.Where(t => returnTargetable is null || Vector3.Distance(t.Position, transform.position) < tempDst))
        {
            returnTargetable = t;
        }

        return returnTargetable;
    }
    #endregion
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + transform.forward * dashSpeed, 3);
        
        Vector3 dir = transform.forward;
        dir.y = 0f;
        
        float minusAngle =-detectionAngle / 2f;
        float maxAngle = detectionAngle / 2f;
        var minusDir = Quaternion.Euler(0f, minusAngle, 0f) * dir;
        var maxDir = Quaternion.Euler(0f, maxAngle, 0f) * dir;
        
        Debug.DrawRay(transform.position, minusDir * detectionDst, Color.blue);
        Debug.DrawRay(transform.position, maxDir * detectionDst, Color.blue);
        Debug.DrawLine(transform.position + (minusDir * detectionDst), transform.position + maxDir * detectionDst, Color.blue);
        Gizmos.color = Color.blue;
        
        if(targetsInLevel.Count == 0) return;

        
        foreach (var t in targetsReachable)
        {
            if (!isLeftTrigger) return;
            var inverseDir = t.Position - transform.position;
            inverseDir.Normalize();
            
            Gizmos.DrawWireSphere(t.Position + (inverseDir * 2), 2);
        }
        
        foreach (var t in targetsInLevel)
        {
            Gizmos.color = targetsReachable.Contains(t) ? Color.green : Color.red;
            Gizmos.DrawLine(transform.position, t.Position);
        }
    }
}
