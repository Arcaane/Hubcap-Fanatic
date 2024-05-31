using System.Collections.Generic;
using System.Linq;
using Helper;
using HubcapCamera;
using UnityEngine;

namespace HubcapManager {
    public class UIIndicatorManager : SingletonUpdatesHandler<UIIndicatorManager>, IUpdate {
        [Header("UI TARGET BASE DATA")]
        [SerializeField] private Transform UiTargetParent = null;
        [SerializeField] private GameObject UITargetObj = null;

        [Header("TARGET DIST DATA")] 
        [SerializeField] private float lerpPosition = 25;
        [SerializeField] private float distFromEdges = 1;
        
        
        private List<UITarget> targets = new();
        private Camera cam = null;
        private Transform cameraCenter = null;

        protected override void Start() {
            base.Start();
            cam = Camera.main;
            cameraCenter = CameraControler.Instance.CameraCenter;
        }

        #region ADD OR REMOVE UITARGET

        /// <summary>
        /// Add an object to the UITarget 
        /// </summary>
        /// <param name="target"></param>
        /// <returns>Return the index of the UITarget</returns>
        public int AddUITargetToList(UITarget target) {
            targets.Add(target);
            CreateUITarget(target);
            return targets.IndexOf(target);
        }

        /// <summary>
        /// Remove a UITarget based on the transform
        /// </summary>
        /// <param name="tr"></param>
        public void RemoveUITargetFromList(Transform tr) {
            UITarget target = FindUITargetBasedOnTransform(tr);
            targets.Remove(target);
            Destroy(target.targetTransform.gameObject);
        }

        /// <summary>
        /// Create a UI Target element on the canvas
        /// </summary>
        /// <param name="target"></param>
        private void CreateUITarget(UITarget target) {
            target.targetTransform = Instantiate(UITargetObj, GetTargetPosition(target), Quaternion.identity, UiTargetParent).transform;
        }

        #endregion ADD OR REMOVE UITARGET
        
        public void UpdateTick() {
            UpdateUITargetsPosition();
        }

        #region UPDATE UITARGET

        /// <summary>
        /// Update all UI Target position based on the object transform
        /// </summary>
        private void UpdateUITargetsPosition() {
            foreach (UITarget target in targets) {
                if (target.targetTransform == null) continue;
                
                target.targetTransform.position = Vector3.Lerp(target.targetTransform.position, GetTargetPosition(target), Time.deltaTime * lerpPosition);
                target.targetTransform.localScale = Vector3.Lerp(target.targetTransform.localScale, GetTargetScale(target), Time.deltaTime * lerpPosition);
            }
        }

        #endregion UPDATE UITARGET

        #region HELPER

        /// <summary>
        /// Is the position in parameter is inside the screen rect
        /// </summary>
        /// <param name="pos"></param>
        /// <returns>Return true if the object is in the screen rect</returns>
        private bool IsInRect(Vector3 pos) => pos is {x: >= 0 and <= 1, y: >= 0 and <= 1};

        /// <summary>
        /// Return the UITarget which contains the same transform as the object in parameter
        /// </summary>
        /// <param name="tr"></param>
        /// <returns></returns>
        private UITarget FindUITargetBasedOnTransform(Transform tr) => targets.FirstOrDefault(target => target.tr == tr);
        
        private float xValue, yValue;
        
        /// <summary>
        /// Get the position of the UITarget in parameter
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private Vector3 GetTargetPosition(UITarget target) {
            Vector3 dir = (target.tr.position - cameraCenter.transform.position).normalized;
            Vector3 screenPos = cam.WorldToScreenPoint(cameraCenter.transform.position + dir);
            Vector2 screenDirFromCenter = (new Vector2(screenPos.x, screenPos.y) - new Vector2(Screen.width / 2, Screen.height / 2)).normalized;
            
            switch (Mathf.Abs(screenDirFromCenter.y) * GetScreenRatio() > Mathf.Abs(screenDirFromCenter.x)) {
                //We know the y value because the point is either at the top or bottom
                case true:
                    yValue = (Screen.height / 2 - distFromEdges) * (screenDirFromCenter.y > 0 ? 1 : -1);
                    xValue = yValue * screenDirFromCenter.x / screenDirFromCenter.y;
                    break;
                    
                //We know the x value because the point is either on the right or left
                case false:
                    xValue = (Screen.width / 2 - distFromEdges) * (screenDirFromCenter.x > 0 ? 1 : -1);
                    yValue =  (screenDirFromCenter.y / screenDirFromCenter.x) * xValue;
                    break;
            }

            return new Vector3(xValue + (Screen.width / 2), yValue + (Screen.height / 2));
        }

        /// <summary>
        /// Get the scale of the UITarget in parameter
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private Vector3 GetTargetScale(UITarget target) => IsInRect(cam.WorldToViewportPoint(target.tr.position)) ? Vector3.zero : Vector3.one;

        /// <summary>
        /// Get the current screen ratio of the player
        /// </summary>
        /// <returns></returns>
        private float GetScreenRatio() => (float) (Screen.width - distFromEdges  * 2) / (Screen.height - distFromEdges  * 2);
        
        #endregion
    }

    [System.Serializable]
    public class UITarget {
        public Transform tr = null;
        public Transform targetTransform = null;
    }
}
