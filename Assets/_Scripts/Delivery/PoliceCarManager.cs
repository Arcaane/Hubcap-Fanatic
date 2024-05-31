using System.Collections.Generic;
using Helper;
using UnityEngine;

namespace HubcapManager {
    public class PoliceCarManager : Singleton<PoliceCarManager> {
        [Header("TARGET DIRECTION")]
        private List<Transform> policeCarTargetPoints = new();
        
        public delegate void TransformListDelegate(List<Transform> targetLists);
        public event TransformListDelegate onPlayerDie = null;
        

        /// <summary>
        /// Method called when the player die
        /// </summary>
        public void CallOnPlayerDeath() => onPlayerDie.Invoke(policeCarTargetPoints);
    }
}