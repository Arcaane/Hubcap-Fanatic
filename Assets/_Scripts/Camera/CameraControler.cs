using Helper;
using HubcapCarBehaviour;
using UnityEngine;

namespace HubcapCamera {
    public class CameraControler : SingletonUpdatesHandler<CameraControler>, IFixedUpdate {
        private PlayerCarController player = null;
        [SerializeField] private float camDistanceFromPlayer = 1;
        [SerializeField] private float distanceFromCenter = 1;
        [SerializeField] private float lerpMultiplier = 5;
        [SerializeField] private Vector3 offsetPosition = new();
        [Space]
        [SerializeField, ReadOnly] private Vector3 playerPosition = new();
        [SerializeField, ReadOnly] private Vector3 targetPosition = new();

        [Header("CAMERA CENTER")]
        [SerializeField] private Transform cameraCenter = null;
        public Transform CameraCenter => cameraCenter;
        
        protected override void Start() {
            base.Start();
            player = PlayerCarController.Instance;
            cameraCenter.transform.localPosition = new Vector3(0, 0, camDistanceFromPlayer);
        }

        public void FixedUpdateTick() {
            playerPosition = player.transform.position;
            targetPosition = playerPosition + Vector3.up * camDistanceFromPlayer;
            transform.position = Vector3.Lerp(transform.position, targetPosition + offsetPosition + player.Rigidbody.velocity * distanceFromCenter, Time.fixedDeltaTime * lerpMultiplier);
        }
    }
}