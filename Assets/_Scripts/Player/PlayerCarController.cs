using System.Collections.Generic;
using Abilities;
using Helper;
using HubcapAbility;
using HubcapManager;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HubcapCarBehaviour {
    [RequireComponent(typeof(CarDeliveryHandler), typeof(ShotgunHandler), typeof(PlayerHealthManager))]
    [RequireComponent(typeof(PlayerExperienceManager))]
    public class PlayerCarController : SingletonCarBehaviour<PlayerCarController> {
        private CarDeliveryHandler delivery = null;
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

        [Header("SHOTGUN")]
        //[SerializeField] private ShotgunCollider shotgunCollider;
        //[SerializeField] private float[] shootTimes;
        [SerializeField] public float shootDuration;
        //[SerializeField] private ParticleSystem shotgunParticles;
        [SerializeField] public int shotgunDamages = 50;
        public bool isStraffing;

        public bool isBomber;
        public bool gotVayneUpgrade;
        public float vaynePassiveMultiplier;
        public float mightPowerUpLevel;

        [Header("Effects")] [SerializeField] public GameObject shield;
        public bool isShield;

        public bool isBerserk;
        public float pillValue;
        public bool overSpeedPill;
        
        
        
        public List<GameObject> pickedItems = new();

        protected override void Start() {
            base.Start();

            mainCamera = Camera.main;
            shotgun = GetComponent<ShotgunHandler>();
            health = GetComponent<PlayerHealthManager>();
            xp = GetComponent<PlayerExperienceManager>();
            delivery = GetComponent<CarDeliveryHandler>();
            
            currentCollsionBeforeDropDeliver = CollsionBeforeDropDeliver;
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
        
        
        
        public int CollsionBeforeDropDeliver = 3;
        private int currentCollsionBeforeDropDeliver;

        protected override void OnCollisionEnter(Collision other) {
            base.OnCollisionEnter(other);
            if (other.gameObject.CompareTag("Wall")) {
                MakeCarBounce(other.contacts, other.relativeVelocity.magnitude);
                CarAbilitiesManager.instance.OnWallCollision.Invoke(other);

                //transform.rotation = Quaternion.Euler(Mathf.Clamp(transform.eulerAngles.x,-maxRotation,maxRotation),transform.eulerAngles.y,Mathf.Clamp(transform.eulerAngles.z,-maxRotation,maxRotation));
            }

            /*if (other.gameObject.CompareTag("Enemy") && pickedItems.Count > 0) {
                currentCollsionBeforeDropDeliver--;

                if (currentCollsionBeforeDropDeliver > 0) return;

                for (int i = 0; i < pickedItems.Count; i++) {
                    ObjectPickable obj = pickedItems[i].GetComponent<ObjectPickable>();
                    obj.OnDrop();
                    obj.rb.AddForce(other.contacts[0].normal.normalized * 100);
                }

                pickedItems.Clear();
                currentCollsionBeforeDropDeliver = CollsionBeforeDropDeliver;
            }*/
        }
        
       

        /*public void XButton(InputAction.CallbackContext context) {
            if (context.started && canShoot() && !isBomber) {
                if (shotgun.enemyCar.Count > 0) {
                    ShotgunHit();
                    //UIManager.Instance.GoodShotUI(shootTimes[0] >= shootDuration ? 0 : 1);
                }
                else {
                    CarAbilitiesManager.instance.OnShotgunUsedWithoutTarget.Invoke();
                    //UIManager.Instance.ShootMissUI(shootTimes[0] >= shootDuration ? 0 : 1);
                }

                for (int i = 0; i < shootTimes.Length; i++) {
                    if (shootTimes[i] >= shootDuration) {
                        shootTimes[i] = 0;
                        break;
                    }
                }

                CarAbilitiesManager.instance.OnShotgunUsed.Invoke();
                CameraShake.instance.SetShake(0.3f);
            }

>>>>>>>>>> CA EN DESSOUS C PAS FAIT
            if (context.started && isBomber && canShoot()) {
                PoolManager.instance.SpawnInstance(ManagerNameSpace.Key.OBJ_Mine, transform.position, Quaternion.identity);

                for (int i = 0; i < shootTimes.Length; i++) {
                    if (shootTimes[i] >= shootDuration) {
                        shootTimes[i] = 0;
                        break;
                    }
                }
            }
        }*/
    }
}