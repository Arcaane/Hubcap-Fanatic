using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    public static CarController instance;
    
    public Rigidbody sphereRB;
    
    public float fwdSpeed;
    public float revSpeed;
    public float turnSpeed;
    public LayerMask groundLayer;

    private float moveInput;
    private float turnInput;
    private bool isCarGrounded;
    
    private float normalDrag;
    public float modifiedDrag;
    
    public float alignToGroundTime;

    private Vector2 moveAxis;
    private bool isDrifting;

    public enum Type
    {
        FullRb,
        SemiRb
    }

    public Type SelectedType = Type.SemiRb;
    
    #region Input Action Events
    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled)
        {
            moveAxis = Vector2.zero;
            return;
        }

        moveAxis = ctx.ReadValue<Vector2>().normalized;
    }
    
    public void OnDrift(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled)
        {
            isDrifting = false;
            return;
        }
        
        isDrifting = true;
    }
    #endregion
    
    void Start()
    {
        if (instance != null)
        {
            instance = this;
        }
        
        // Detach Sphere from car
        sphereRB.transform.parent = null;
        normalDrag = sphereRB.drag;
    }
    
    [Space(5)]
    private Vector3 CarVelocity;
    private float dragStrenght = 0.98f;
    public float steerAngle = 20f;
    public int traction = 20;
    public float moveSpeed;
    public float maxMoveSpeed;
    public AnimationCurve steerCurve;
    
    void Update()
    {
        moveInput = moveAxis.y;
        turnInput = moveAxis.x;

        if (SelectedType == Type.FullRb)
        {
            // Calculate Turning Rotation
            float steer = moveAxis.x;
            sphereRB.transform.Rotate(Vector3.up * (steer * CarVelocity.magnitude * steerAngle * Time.deltaTime));
            if (isCarGrounded) transform.Rotate(Vector3.up * (steer * steerCurve.Evaluate(sphereRB.velocity.magnitude) * steerAngle * Time.deltaTime), Space.World);
            
            Debug.Log(sphereRB.velocity.magnitude);
            
            
            // Set Cars Position to Our Sphere
            transform.position = sphereRB.transform.position;

            // Raycast to the ground and get normal to align car with it.
            RaycastHit hit;
            isCarGrounded = Physics.Raycast(transform.position, -transform.up, out hit, 1f, groundLayer);
        
            // Rotate Car to align with ground
            Quaternion toRotateTo = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotateTo, alignToGroundTime * Time.deltaTime);
        
            // Calculate Movement Direction
            moveInput *= moveInput > 0 ? fwdSpeed : revSpeed;
        
            // Calculate Drag
            sphereRB.drag = isCarGrounded ? normalDrag : modifiedDrag;
        
            Debug.DrawRay(transform.position,  (transform.forward * moveInput).normalized);
            Debug.DrawRay(transform.position, transform.forward * 2, Color.blue);
        }
        else
        {
            // Moving
            CarVelocity *= moveAxis.y * moveSpeed * Time.deltaTime;
            //transform.position += CarVelocity * Time.deltaTime;

            // Steering
            float steer = moveAxis.x;
            sphereRB.transform.Rotate(Vector3.up * (steer * CarVelocity.magnitude * steerAngle * Time.deltaTime));
            
            transform.position = sphereRB.transform.position;
            
            RaycastHit hit;
            isCarGrounded = Physics.Raycast(transform.position, -transform.up, out hit, 1f, groundLayer);
            
            /*
            // Drag 
            CarVelocity *= dragStrenght;
            CarVelocity = Vector3.ClampMagnitude(CarVelocity, maxMoveSpeed); */
            
            // Calculate Drag
            sphereRB.drag = isCarGrounded ? normalDrag : modifiedDrag;
        
            // Traction
            Debug.DrawRay(transform.position, CarVelocity.normalized * 2);
            Debug.DrawRay(transform.position, transform.forward * 2, Color.blue);
            CarVelocity = Vector3.Lerp(CarVelocity.normalized, transform.forward, traction * Time.deltaTime) * CarVelocity.magnitude;
        }
    }

    private void FixedUpdate()
    {
        if (SelectedType == Type.FullRb)
        {
            if (isCarGrounded) sphereRB.AddForce(transform.forward * moveInput, ForceMode.Acceleration);
            else sphereRB.AddForce(transform.up * -200f);
        }
        else
        {
            if (isCarGrounded)
            {
                sphereRB.AddForce(Vector3.Lerp(CarVelocity.normalized, transform.forward, traction * Time.deltaTime) * CarVelocity.magnitude, ForceMode.Force);
            }
            else sphereRB.AddForce(transform.up * -200f);
        }
    }
}
