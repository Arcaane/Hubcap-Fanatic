using UnityEngine;

namespace HubcapCamera {
    public class SmoothCameraRotationWithMouse : UpdatesHandler, IUpdate {
        [SerializeField] private Vector2 rotationSpeed = new();
        [SerializeField, Range(0, 1)] private float lerpSpeed = 0.5f;
        [SerializeField, Range(0, 1)] private float lerpSpeedGamepad = 0.5f;
        private bool useKeyboard = true;
        private Camera cam;
        private Vector3 startRotation = new();
        private Vector2 dir = new();
        
        /// <summary>
        /// Initialize data for the camera rotation
        /// </summary>
        private void Awake() {
            cam = Camera.main;
            startRotation = cam.transform.eulerAngles;
        }

        protected override void OnStartContinue() {
            base.OnStartContinue();
            InputManager.Instance.OnRightStickMove += MoveCamera;
            InputManager.Instance.OnInputChange += SwitchController;
        }

        protected override void OnDisableContinue() {
            base.OnDisableContinue();
            InputManager.Instance.OnRightStickMove -= MoveCamera;
            InputManager.Instance.OnInputChange -= SwitchController;
        }

        /// <summary>
        /// Switch the controller type
        /// </summary>
        /// <param name="isKeyboard"></param>
        private void SwitchController(object isKeyboard) => useKeyboard = (bool)isKeyboard;
        
        /// <summary>
        /// Move the camera with the right joystick
        /// </summary>
        /// <param name="direction"></param>
        private void MoveCamera(object direction) => dir = new Vector2(((Vector2)direction).x * (Screen.width / 2) * rotationSpeed.x, ((Vector2)direction).y * (Screen.height / 2) * -rotationSpeed.y);

        /// <summary>
        /// Update the rotation of the camera based on the position of the mouse
        /// </summary>
        public void UpdateTick() {
            if(useKeyboard) dir = new Vector2((Input.mousePosition.x - Screen.width / 2) * rotationSpeed.x, (Input.mousePosition.y - Screen.height / 2) * -rotationSpeed.y);
            cam.transform.localRotation = Quaternion.Lerp(cam.transform.localRotation, Quaternion.Euler(startRotation.x + dir.y, startRotation.y + dir.x, 0), useKeyboard ? lerpSpeed : lerpSpeedGamepad);
        }
    }
}