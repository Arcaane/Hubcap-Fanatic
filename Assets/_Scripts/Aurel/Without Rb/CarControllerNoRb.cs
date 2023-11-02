using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CarControllerNoRb : MonoBehaviour
{
    public static CarControllerNoRb instance;
    
    public GameObject[] wheelsGO;

    [FormerlySerializedAs("baseMoveSpeed")] [Space(3)][Header("Movement")]
    public float accelerationSpeed;
    public float maxMoveSpeed;

    [Space(3)][Header("Steering")]
    public Vector2 steerAnglesValues;

    [Space(3)][Header("Draging")]
    public float dragStrenght = 0.98f;

    [Space(3)][Header("Traction")]
    public Vector2 tractionValues;

    private Vector2 moveAxis;
    private float steerAngle; 
    private float traction;
    private Vector3 CarVelocity;
    private Vector3 cachedPosition;
    private Vector3 _forward;

    [Space(8)][Header("Dash")]
    public DashTriggerCondition dashTriggerCondition = DashTriggerCondition.OnTrigger;
    public float timeToAddDrift;
    public float dashDuration, dashDst;
    public float maxPower = 100;
    public float power;
    public Image powerUI;

    private float pTimer;
    private float timer;
    private bool isDashing;
    private bool isDrifting;
    
    public enum DashTriggerCondition { OnRealease, OnTrigger}
    
    #region Input Action Events
    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled)
        {
            moveAxis.x = 0;
            return;
        }

        moveAxis.x = ctx.ReadValue<Vector2>().normalized.x;
    }
    
    public void OnDrift(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled)
        {
            isDrifting = false;
            pTimer = 0;
            //Todo - Suppr
            //Gamepad.current.SetMotorSpeeds(0, 0);
            return;
        }
        
        isDrifting = true;
    }

    public void OnAccelerate(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled)
        {
            moveAxis.y = 0;
            return;
        }

        moveAxis.y = ctx.ReadValue<float>();
    }
    
    public void OnBrakeOrMoveBack(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled)
        {
            moveAxis.y = 0;
            return;
        }
        
        moveAxis.y = -ctx.ReadValue<float>();
    }

    public void OnDash(InputAction.CallbackContext ctx)
    {
        if (dashTriggerCondition == DashTriggerCondition.OnTrigger)
        {
            Dash();
        }

        if (dashTriggerCondition == DashTriggerCondition.OnRealease && ctx.canceled)
        {
            Dash();
        }
    }
    #endregion

    private void Start()
    {
        instance = this;
    }

    public void Update()
    {
        Ride();

        if (!isDrifting) return;
        
        AddPowerWhenDrift();
    }
    
    private void OnDrawGizmos()
    {
        Vector3 initPos = transform.position;
        Vector3 finalPos = initPos + transform.forward * dashDst;
        
        Gizmos.DrawWireSphere(finalPos, 1);
    }

    private void Ride()
    {
        if (isDashing) return;
        cachedPosition = transform.position;
        traction = isDrifting ? tractionValues.x : tractionValues.y;
        steerAngle = isDrifting ? steerAnglesValues.x : steerAnglesValues.y;
        
        // Moving
        CarVelocity += transform.forward * accelerationSpeed * moveAxis.y * Time.deltaTime;
        transform.position += CarVelocity * Time.deltaTime;
        
        // Draging
        CarVelocity *= dragStrenght;
        CarVelocity = Vector3.ClampMagnitude(CarVelocity, maxMoveSpeed);
        
        // Steering
        transform.Rotate(Vector3.up * moveAxis.x * CarVelocity.magnitude * steerAngle * Time.deltaTime);
        
        // Traction
        Debug.DrawRay(cachedPosition, CarVelocity.normalized * 2);
        Debug.DrawRay(cachedPosition, transform.forward * 2, Color.blue);
        //CarVelocity = Vector3.Lerp(CarVelocity.normalized, transform.forward * moveAxis.y, traction * Time.deltaTime) * CarVelocity.magnitude;

        wheelsGO[0].SetActive(isDrifting);
        wheelsGO[1].SetActive(isDrifting);
    }
    
    private void AddPowerWhenDrift()
    {
        pTimer += Time.deltaTime;
        if (pTimer > timeToAddDrift)
        {
            //Todo - Suppr
            //Gamepad.current.SetMotorSpeeds(0.25f, 0.5f);
            if (power >= maxPower) return;
            GivePowerByPercent(0.05f);
            powerUI.fillAmount = power / maxPower;
        }
    }
    
    public void GivePowerByNumber(float power)
    {
        if (this.power >= maxPower) return;
        
        this.power += power;
        
        if (this.power >= maxPower)
        {
            this.power = maxPower;
            PowerFull();
        }
    }

    public void GivePowerByPercent(float percentToAdd)
    {
        if (power >= maxPower) return;
        
        float temp = percentToAdd / 100f;
        power += (temp * maxPower);
        powerUI.fillAmount = power / maxPower;
        
        if (power >= maxPower)
        {
            power = maxPower;
            PowerFull();
        }
    }

    private async void PowerFull()
    {
        var baseColor = powerUI.color;
        powerUI.color = Color.white;
        await Task.Delay(225);
        powerUI.color = baseColor;
    }
    private async void Dash()
    {
        if (power != maxPower) return;
        isDashing = true;
        
        Vector3 initPos = transform.position;
        Vector3 finalPos = initPos + transform.forward * dashDst;
        
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
        power = 0;
        powerUI.fillAmount = power / maxPower;
        isDashing = false;
    }
}
