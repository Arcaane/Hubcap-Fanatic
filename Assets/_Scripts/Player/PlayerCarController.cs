using Abilities;
using Helper;
using HubcapAbility;
using HubcapManager;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HubcapCarBehaviour {
    [RequireComponent(typeof(ShotgunHandler), typeof(PlayerHealthManager))]
    [RequireComponent(typeof(PlayerExperienceManager))]
    public class PlayerCarController : SingletonCarBehaviour<PlayerCarController> {
        private PlayerHealthManager health = null;
        private PlayerExperienceManager xp = null;
        private ShotgunHandler shotgun = null;
        private Camera mainCamera = null;
        public Camera uiCamera => mainCamera.transform.GetChild(0).GetComponent<Camera>();
        public PlayerHealthManager playerHealthManager => health;
        public PlayerExperienceManager playerExperienceManager => xp;

        [Header("NITRO")] 
        [SerializeField] public float speedWithNitro = 50;
        [SerializeField] private float nitroDuration = 2;
        [SerializeField, Range(0, 1)] public float nitroRegen = 0.1f;
        [SerializeField] private float minNitroEnable = 0.25f;
        [SerializeField, ReadOnly] private bool isUsingNitro = false;
        [HideInInspector] public bool canNitro = true;
        public bool IsUsingNitro => isUsingNitro;
        private float currentNitroAmount;
        [Space] 
        [SerializeField] private ParticleSystem smoke = null;
        [SerializeField] private ParticleSystem smokeNitro = null;


        [HideInInspector] public bool isBomber;
        [HideInInspector] public bool gotVayneUpgrade;
        [HideInInspector] public float vaynePassiveMultiplier;
        [HideInInspector] public float mightPowerUpLevel;

        [Header("Effects")] [SerializeField] public GameObject shield;
        [HideInInspector] public bool isShield;
        [HideInInspector] public bool isBerserk;

        protected override void Start() {
            base.Start();

            mainCamera = Camera.main;
            shotgun = GetComponent<ShotgunHandler>();
            health = GetComponent<PlayerHealthManager>();
            xp = GetComponent<PlayerExperienceManager>();
            shield.SetActive(false);
            isShield = false;
            
            InGameUIManager.Instance.InitNitroSlider(minNitroEnable);
            InitInputEvents();
        }

        #region INPUT METHODS
        /// <summary>
        /// Add methods to the InputManager
        /// </summary>
        private void InitInputEvents() {
            InputManager.Instance.Inputs.Player.Move.performed += UpdateDirectionInput;
            InputManager.Instance.Inputs.Player.Move.canceled += UpdateDirectionInput;
            
            InputManager.Instance.Inputs.Player.Accelerate.performed += UpdateAccelerationInput;
            InputManager.Instance.Inputs.Player.Accelerate.canceled += UpdateAccelerationInput;
            
            InputManager.Instance.Inputs.Player.Brake.performed += UpdateBrakeInput;
            InputManager.Instance.Inputs.Player.Brake.canceled += UpdateBrakeInput;
            
            InputManager.Instance.Inputs.Player.Nitro.started += UpdateNitroState;
            InputManager.Instance.Inputs.Player.Nitro.canceled += UpdateNitroState;
        }

        /// <summary>
        /// Remove methods from the InputManager when this object is disabled
        /// </summary>
        protected override void OnDisable() {
            base.OnDisable();
            
            InputManager.Instance.Inputs.Player.Move.performed -= UpdateDirectionInput;
            InputManager.Instance.Inputs.Player.Move.canceled -= UpdateDirectionInput;
            
            InputManager.Instance.Inputs.Player.Accelerate.performed -= UpdateAccelerationInput;
            InputManager.Instance.Inputs.Player.Accelerate.canceled -= UpdateAccelerationInput;
            
            InputManager.Instance.Inputs.Player.Brake.performed -= UpdateBrakeInput;
            InputManager.Instance.Inputs.Player.Brake.canceled -= UpdateBrakeInput;
            
            InputManager.Instance.Inputs.Player.Nitro.started -= UpdateNitroState;
            InputManager.Instance.Inputs.Player.Nitro.canceled -= UpdateNitroState;
        }

        /// <summary>
        /// Method called when the left stick is moved (direction value)
        /// </summary>
        /// <param name="context"></param>
        private void UpdateDirectionInput(InputAction.CallbackContext context) => directionXInput = context.performed ? context.ReadValue<float>() : 0;

        /// <summary>
        /// Method called when the right shoulder is pressed (acceleration value)
        /// </summary>
        /// <param name="context"></param>
        private void UpdateAccelerationInput(InputAction.CallbackContext context) => accelerationInput = context.performed ? context.ReadValue<float>() : 0;
        
        /// <summary>
        /// Method called when the left shoulder is pressed (brake value)
        /// </summary>
        /// <param name="context"></param>
        private void UpdateBrakeInput(InputAction.CallbackContext context) {
            brakeInput = context.performed ? context.ReadValue<float>() : 0;

            if (isDrifting) return;
            
            if (context.started && (directionXInput > -minJoystickValueToStartDrift || directionXInput < minJoystickValueToStartDrift)) {
                CarAbilitiesManager.instance.OnPill.Invoke();
            }
            if (context.performed && (directionXInput > minJoystickValueToStartDrift || directionXInput < -minJoystickValueToStartDrift)) {
                StartDrifting();
            }
        }
        
        /// <summary>
        /// Method called when the button to nitro is pressed
        /// </summary>
        /// <param name="context"></param>
        private void UpdateNitroState(InputAction.CallbackContext context) {
            if (context.started && canNitro) StartUsingNitro();
            if (context.canceled && isUsingNitro) StopUsingNitro();
        }

        #endregion INPUT METHODS


        public override void UpdateTick() {
            base.UpdateTick();

            if(health.IsAlive) OnMovementUpdate();
            
            CheckDriftState();
            CheckNitroState();
            FillNitroBackIfRequired();
            
            /*if (isShield) {
                shield.transform.position = transform.position;
            }*/
        }
        
        public override void FixedUpdateTick() {
            base.FixedUpdateTick();
            
            ChangeSpeedBasedOnRoadUnderPlayer();
        }

        
        #region NITRO METHODS

        /// <summary>
        /// Make the player start using nitro
        /// </summary>
        private void StartUsingNitro() {
            isUsingNitro = true;
            canNitro = false;
            
            smoke.Stop();
            smokeNitro.Play();
            InGameUIManager.Instance.StartNitroEffect();

            targetSpeed = speedWithNitro;
            CarAbilitiesManager.instance.OnBeginNitro.Invoke();
        }

        /// <summary>
        /// Check the current status of the nitro and switch state if required
        /// </summary>
        private void CheckNitroState() {
            if (!isUsingNitro) return;

            if (currentNitroAmount <= 0) {
                StopUsingNitro();
                return;
            }

            currentNitroAmount -= Time.deltaTime;
            InGameUIManager.Instance.UpdateNitroSlider(currentNitroAmount / nitroDuration);

            //CarAbilitiesManager.instance.OnUpdate.Invoke();
        }

        /// <summary>
        /// Fill the nitro when the player is not trying to use it
        /// </summary>
        private void FillNitroBackIfRequired() {
            if (currentNitroAmount >= nitroDuration || isUsingNitro) return;
            if (currentNitroAmount >= minNitroEnable) canNitro = true;

            currentNitroAmount += Time.deltaTime * nitroRegen;
            InGameUIManager.Instance.UpdateNitroSlider(currentNitroAmount / nitroDuration);
        }
        
        /// <summary>
        /// Stop the player from being able to use the nitro
        /// </summary>
        private void StopUsingNitro() {
            isUsingNitro = false;
            
            smoke.Play();
            smokeNitro.Stop();
            InGameUIManager.Instance.StopNitroEffect();
            
            targetSpeed = maxRoadSpeed;
            CarAbilitiesManager.instance.OnStateExit.Invoke();
        }
        
        #endregion NITRO METHODS

        #region DRIFT METHODS

        /// <summary>
        /// Make the player starts drifting
        /// </summary>
        private void StartDrifting() {
            isDrifting = true;
            CarAbilitiesManager.instance.OnBeginDrift.Invoke();
            foreach (ParticleSystem sparks in driftSparks) sparks.Play();
        }

        /// <summary>
        /// Check the current status of the drift and switch it based on condition
        /// </summary>
        private void CheckDriftState() {
            if (!isDrifting || accelerationInput > 0.1f) return;
            StopDrifting();
        }

        /// <summary>
        /// Make the player stop drifting
        /// </summary>
        private void StopDrifting() {
            isDrifting = false;
            CarAbilitiesManager.instance.OnStateExit.Invoke();
            foreach (ParticleSystem sparks in driftSparks) sparks.Stop();
        }

        #endregion DRIFT METHODS

        /// <summary>
        /// Change the targetSpeed of the player based on the road under the car
        /// </summary>
        private void ChangeSpeedBasedOnRoadUnderPlayer() {
            if (isUsingNitro) return;
            
            if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, 10f, roadMask)) {
                targetSpeed = maxRoadSpeed;
            }
            else {
                targetSpeed = offRoadSpeed;
                if (speedFactor > 1) rb.velocity = Vector3.Lerp(rb.velocity, Vector3.ClampMagnitude(rb.velocity, targetSpeed), Time.fixedDeltaTime * offRoadDeccelerationFactor);
            }
        }
        
        protected override void OnCollisionEnter(Collision other) {
            base.OnCollisionEnter(other);
            if (other.gameObject.CompareTag("Wall")) {
                MakeCarBounce(other.contacts, other.relativeVelocity.magnitude);
                CarAbilitiesManager.instance.OnWallCollision.Invoke(other);
            }
        }
    }
}