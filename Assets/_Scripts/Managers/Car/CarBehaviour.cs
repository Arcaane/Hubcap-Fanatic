using System;
using HubcapManager;
using UnityEngine;
using UnityEngine.Events;

namespace HubcapCarBehaviour {
    [RequireComponent(typeof(Rigidbody))]
    public class CarBehaviour : UpdatesHandler, IUpdate, IFixedUpdate {
        protected Rigidbody rb = null;
        public Rigidbody Rigidbody => rb;

        [Header("WHEEL INFORMATION")] 
        [SerializeField] public Wheel[] wheels;
        [SerializeField, Range(0, 1)] private float wheelYPos = 0.5f;
        [SerializeField] private float wheelMass = 0.1f;

        [Header("CAR BASE DATA")] 
        [SerializeField] private float carHeightPos = 0.9f;
        [SerializeField] private Vector3 localCenterOfMass;
        [SerializeField] private LayerMask allGroundLayers = new();

        [Header("MOVEMENT SPEED")] 
        [SerializeField] private float acceleration = 12f;
        [SerializeField] private AnimationCurve accelerationBySpeedFactor;
        [SerializeField, ReadOnly, Tooltip("This is the value of the input to accelerate")] protected float accelerationInput = 0f;
        [ReadOnly, Tooltip("this is the speed the car is trying to reach in game")] public float targetSpeed = 25f;
        [Space]
        [SerializeField] protected LayerMask roadMask = new();
        [SerializeField] public float maxRoadSpeed = 25f;
        [Space]
        [SerializeField] public float offRoadSpeed = 10;
        [SerializeField] protected float offRoadDeccelerationFactor = 0.625f;

        [Header("BRAKE / DECELERATION")]
        [SerializeField] private float braking = 12f;
        [SerializeField] private AnimationCurve brakingBySpeedFactor = new();
        [SerializeField] private float deceleration = 4f;
        [SerializeField, ReadOnly, Tooltip("This is the value of the input to brake")] protected float brakeInput = 0f;

        [Header("CAR STEERING")] 
        [SerializeField] private float steeringSpeed = 50f;
        [SerializeField] private AnimationCurve steeringBySpeedFactor = new();
        [Tooltip("This is the X value of the direction input")]
        [SerializeField, ReadOnly] protected float directionXInput = 0f;

        [Header("BRAKE / DRIFT")] 
        [Tooltip("This is a boolean which allow to know if the player is currently drifting")]
        [ReadOnly] public bool isDrifting = false;
        [SerializeField] private float brakeDampeningMultiplier = 0.25f;
        [SerializeField] private float brakeSteeringMultiplier = 0.95f;
        [SerializeField] private float brakeAccelerationMultiplier = 1.5f;
        [Space] 
        [SerializeField] protected float minJoystickValueToStartDrift = 0.6f;
        [SerializeField] private float minAngleToExitDrift = 0.1f;
        [SerializeField] protected ParticleSystem[] driftSparks;
        
        [Header("CAR BOUNCE")]
        [Tooltip("The percent of speed retained after bounce")] 
        [SerializeField] public float speedRetained = 0.7f;
        [Tooltip("The min angle to bounce (a value of 1 if is equal to 90°)")] 
        [SerializeField] private float minDotAngleToBounce = 0.3f;
        [Tooltip("The min dot angle to deal damages to the other car (a value of 1 if is equal to 90°)")] 
        [SerializeField] protected float minDotAngleToDealDamage = 0.9f;
        [SerializeField, Pooler] private string fxBounceKey = "";
        [HideInInspector] public UnityEvent<Collision> OnColliderEnter = new();

        [Header("Effects applied")] public bool forceBreak;
        public float forceBreakTimer;
        private bool isScorch;
        public bool IsScorch => isScorch;
        [SerializeField] protected ParticleSystem fxFire = null;
        public bool isAimEffect;
        
        public float speedFactor => rb.velocity.magnitude / targetSpeed;
        public float globalSpeedFactor => rb.velocity.magnitude / maxRoadSpeed;


        private bool driftEngaged;
        private float driftValue;


        #region INIT
        
        protected override void Start() {
            base.Start();

            InitRigidbody();
            UpdateCarYPosition();
            UpdateWheelsPosition();
        }

        /// <summary>
        /// Initialize the value for the rigidbody
        /// </summary>
        private void InitRigidbody() {
            rb = GetComponent<Rigidbody>();
            rb.centerOfMass = localCenterOfMass;
        }
        
        #endregion INIT
        
        #region UPDATE METHODS
        
        /// <summary>
        /// Update method called in the UpdateManager
        /// </summary>
        public virtual void UpdateTick() {
            if (forceBreak) {
                forceBreakTimer -= Time.deltaTime;
                if (forceBreakTimer <= 0) forceBreak = false;
            }
        }

        /// <summary>
        /// Fixed Update method called in the UpdateManager
        /// </summary>
        public virtual void FixedUpdateTick() => UpdateForceOnWheels();
        
        #endregion UPDATE METHODS


        /// <summary>
        /// Update the position of the car on the y axis
        /// </summary>
        [ContextMenu("Update car Height")]
        private void UpdateCarYPosition() {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, allGroundLayers)) {
                transform.position = new Vector3(transform.position.x, hit.point.y + carHeightPos, transform.position.z);
            }
        }


        /// <summary>
        /// Update some data and visuals when the car is moving
        /// </summary>
        protected void OnMovementUpdate() {
            UpdateWheelsRotation();
            UpdateDriftData();
        }

        private void UpdateDriftData() {
            driftValue = 1 - Mathf.Abs(Vector3.Dot(new Vector3(rb.velocity.normalized.x, 0, rb.velocity.normalized.z), transform.forward));
            
            if (!isDrifting) return;

            if (!driftEngaged && driftValue >= minAngleToExitDrift) {
                driftEngaged = true;
            }
            else if (driftEngaged && driftValue < minAngleToExitDrift) {
                isDrifting = false;
                driftEngaged = false;

                if (driftSparks.Length == 0) return;
                foreach (ParticleSystem sparks in driftSparks) sparks.Stop();
            }
        }


        #region WHEELS METHODS
        
        /// <summary>
        /// Method which allow to make the car move based on the forces applied to the wheels
        /// </summary>
        private void UpdateForceOnWheels() {
            foreach (Wheel wheel in wheels) {
                Vector3 wheelForce = GetWheelForces(wheel, wheel.socketTransform.right, wheel.socketTransform.forward);

                if (wheelForce.magnitude <= 0.1f) continue;
                rb.AddForceAtPosition(wheelForce, wheel.socketTransform.position);
            }
        }

        /// <summary>
        /// Update the position of the car's wheels
        /// </summary>
        [ContextMenu("Update wheels position")]
        private void UpdateWheelsPosition() {
            foreach (Wheel wheel in wheels) {
                wheel.wheelVisualTransform.position = wheel.socketTransform.position - wheel.socketTransform.up * (1 - wheelYPos);
            }
        }

        /// <summary>
        /// Update the current rotation of the wheels
        /// </summary>
        private void UpdateWheelsRotation() {
            foreach (Wheel wheel in wheels) {
                if (wheel.steeringFactor <= 0) continue;

                Quaternion updatedRotation = Quaternion.Euler(0, directionXInput * wheel.steeringFactor * GetSteeringBasedOnSpeed() * (isDrifting ? brakeSteeringMultiplier : 1), 0);
                wheel.wheelVisualTransform.localRotation = Quaternion.Lerp(wheel.wheelVisualTransform.localRotation, updatedRotation, Time.deltaTime * steeringSpeed);
                wheel.socketTransform.localRotation = Quaternion.Lerp(wheel.wheelVisualTransform.localRotation, updatedRotation, Time.deltaTime * steeringSpeed);
            }
        }

        /// <summary>
        /// Get the force vector to apply to the wheel in parameter
        /// </summary>
        /// <param name="wheel"></param>
        /// <returns></returns>
        private Vector3 GetWheelForces(Wheel wheel, Vector3 right, Vector3 forward) {
            return right * GetWheelDampening(wheel) + forward * (wheel.drivingFactor > 0 ? GetWheelAcceleration(wheel) : 0);
        }

        /// <summary>
        /// Get the dampening of the wheel (which will serve as the direction vector based on the wheel rotation)
        /// </summary>
        /// <param name="wheel"></param>
        /// <returns></returns>
        private float GetWheelDampening(Wheel wheel) {
            Vector3 wheelWorldVelocity = rb.GetPointVelocity(wheel.socketTransform.position);
            float tangentSpeed = Vector3.Dot(wheelWorldVelocity, wheel.socketTransform.right);
            float counterAcceleration = (-tangentSpeed * wheel.directionalDampening) / Time.fixedDeltaTime;

            return wheelMass * counterAcceleration * (isDrifting ? brakeDampeningMultiplier : 1);
        }

        /// <summary>
        /// Get the acceleration to apply to the wheel (which will serve as the forward vector for the car)
        /// </summary>
        /// <param name="wheel"></param>
        /// <returns></returns>
        private float GetWheelAcceleration(Wheel wheel) {
            if (forceBreak) return GetBrakeFactor(wheel);

            float accel = accelerationInput * acceleration * GetAccelerationBasedOnSpeed() * wheel.drivingFactor * (isDrifting ? brakeAccelerationMultiplier : 1);
            
            float brake = 0;
            /*if (Vector3.Dot(rb.velocity, transform.forward) > 0.1f) brake = brakeInput * GetBrakingFactor(wheel);
            else brake = brakeInput * -deceleration * wheel.drivingFactor;*/
            brake = brakeInput * GetBrakeFactor(wheel);
            
            return accel + brake;
        }

        #endregion WHEELS METHODS

        #region BOUNCE METHOD
        
        /// <summary>
        /// Make the car bounce when car enter in collision with the specific elements
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="relativeVelocityMagnitude"></param>
        protected void MakeCarBounce(ContactPoint[] contact, float relativeVelocityMagnitude) {
            if (!(Vector3.Dot(contact[0].normal, transform.forward) < -minDotAngleToBounce)) return;
            
            Vector2 reflect = Vector2.Reflect(new Vector2(transform.forward.x, transform.forward.z), new Vector2(contact[0].normal.x, contact[0].normal.z));
            transform.forward = new Vector3(reflect.x, 0, reflect.y);
            rb.velocity = transform.forward * relativeVelocityMagnitude * speedRetained;
            rb.angularVelocity = Vector3.zero;

            /*for (int i = 0; i < wheels.Length; i++) {
                if (wheels[i].steeringFactor > 0) {
                    wheels[i].wheelVisualTransform.localRotation = wheels[i].socketTransform.localRotation = Quaternion.Euler(0, 0, 0);
                }
            }*/

            PoolManager.Instance.RetrieveOrCreateObject(fxBounceKey, contact[0].point, Quaternion.LookRotation(contact[0].normal));
        }

        /// <summary>
        /// When the car enter in collision
        /// </summary>
        /// <param name="other"></param>
        protected virtual void OnCollisionEnter(Collision other) => OnColliderEnter?.Invoke(other);

        #endregion

        #region SCORCH

        public virtual void EnableCarScorch() {
            isScorch = true;
        }

        public virtual void DisableCarScorch() {
            isScorch = false;
        }
        
        #endregion SCORCH

        #region HELPER METHODS

        /// <summary>
        /// Get the steering multiplier based on the current speed of the car
        /// </summary>
        /// <returns></returns>
        protected float GetSteeringBasedOnSpeed() => steeringBySpeedFactor.Evaluate(globalSpeedFactor);

        /// <summary>
        /// Get the acceleration multiplier based on the current speed of the car
        /// </summary>
        /// <returns></returns>
        private float GetAccelerationBasedOnSpeed() => accelerationBySpeedFactor.Evaluate(speedFactor);

        /// <summary>
        /// Get the braking factor based on multiple factors
        /// </summary>
        /// <param name="wheel"></param>
        /// <returns></returns>
        private float GetBrakeFactor(Wheel wheel) => -braking * wheel.drivingFactor * (isDrifting ? 0 : 1) * brakingBySpeedFactor.Evaluate(speedFactor);
        
        #endregion HELPER METHODS
    }


    [Serializable]
    public struct Wheel {
        public Transform socketTransform;
        public Transform wheelVisualTransform;
        public float directionalDampening;
        public float drivingFactor;
        public float steeringFactor;
    }
}