using System.Collections.Generic;
using HubcapManager;
using UnityEngine;

namespace HubcapCarBehaviour {
    public class PlayerExperienceManager : MonoBehaviour {
        [Header("LEVELS DATA")]
        [SerializeField, ReadOnly] private int currentLevel = 0;
        [SerializeField, ReadOnly] private int currentExperienceAmount = 0;
        [SerializeField, ReadOnly] private int currentShopTokenAvailable = 0;
        [SerializeField] private List<ExperienceData> xpRequiredPerLevel = new();
        [SerializeField] private int healBackPerLevel = 10;
        public int CurrentLevel => currentLevel;
        
#if UNITY_EDITOR
        private void OnValidate() => UpdateXpAmountToReach();
#endif

        private void Start() {
            currentLevel = 0;
            currentExperienceAmount = 0;
            UpdateExperienceVisuals();
        }

        #region EXPERIENCE/LEVEL
        
        /// <summary>
        /// Add an amount of experience to the player
        /// </summary>
        /// <param name="amount">The amount of experience to add</param>
        public void GetPlayerExperience(int amount) {
            currentExperienceAmount += amount;
            UpdateExperienceVisuals();
            CheckForLevelUp();
        }

        /// <summary>
        /// Check if the player need to level up
        /// </summary>
        private void CheckForLevelUp() {
            if (currentExperienceAmount < xpRequiredPerLevel[currentLevel].previousXpSum) return;
            LevelUp();
        }
        
        /// <summary>
        /// Make the player level up
        /// </summary>
        private void LevelUp() {
            currentLevel++;
            UpdateExperienceVisuals();
            PlayerCarController.Instance.playerHealthManager.HealCar(healBackPerLevel);

            AddShopToken(1);

            CheckForLevelUp();
        }

        #endregion EXPERIENCE/LEVEL
        
        /// <summary>
        /// Update all visuals related to experience and level
        /// </summary>
        private void UpdateExperienceVisuals() {
            InGameUIManager.Instance.UpdateLevelSlider(GetPercentToNextLevel(), currentLevel + 1);
        }

        /// <summary>
        /// Add a token which allow to get a new ability in the shop
        /// </summary>
        /// <param name="amount">The number of token to add for the shop</param>
        public void AddShopToken(int amount) {
            currentShopTokenAvailable += amount;
            /* open shop */   
        }

        #region HELPER
        
        /// <summary>
        /// Update the previousXpSum value of each element of the xpRequired list
        /// </summary>
        private void UpdateXpAmountToReach() {
            for (int index = 0; index < xpRequiredPerLevel.Count; index++) {
                xpRequiredPerLevel[index].previousXpSum = XpAmountToReachLevel(index);
            }
        }
        
        /// <summary>
        /// Return the amount of xp to have to reach the next level
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private int XpAmountToReachLevel(int level) => xpRequiredPerLevel[level].xpRequired + (level != 0 ? xpRequiredPerLevel[level - 1].previousXpSum : 0);
        
        /// <summary>
        /// Return the percent value (between 0 and 1) of the experience to reach the next level
        /// </summary>
        /// <returns></returns>
        private float GetPercentToNextLevel() => (currentExperienceAmount - (currentLevel != 0 ? XpAmountToReachLevel(currentLevel - 1) : 0)) / (float) XpAmountToReachLevel(currentLevel);

        #endregion HELPER
    }

    [System.Serializable]
    public class ExperienceData {
        public int xpRequired = 0;
        public int previousXpSum = 0;
    }
}