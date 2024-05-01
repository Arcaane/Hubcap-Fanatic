using System.Collections.Generic;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine;
using Helper;
using HubcapLocalisation;

namespace HubcapManager {
    public class GameManager : Singleton<GameManager> {
        [SerializeField] private SaveDataHandler save = null;

        [Header("Saved Data")]
        [SerializeField] private bool useEncryptedData = false;
        [SerializeField] private int goldAmount = 0;
        private SavedData data = null;
        public bool UseEncryptedData => useEncryptedData;

        [Header("Scene Transition")] 
        [SerializeField] private CanvasGroup transitionCanvas = null;

        [Header("Scene Transition")]
        [SerializeField] private List<TextAsset> localisationAssets = new();
        [SerializeField] private string currentLanguage = "";
        
        protected override void AwakeContinue() {
            base.AwakeContinue();
            InitData();
            SceneManager.sceneLoaded += (_, _) => {
                transitionCanvas.DOKill();
                transitionCanvas.DOFade(0, 1f);
            };
            LocalizationManager.ReadData(localisationAssets);
        }

        #region DATA MANAGER
        /// <summary>
        /// Load data and store them here
        /// </summary>
        private void InitData() {
            data = save.LoadGameData();
            goldAmount = data.goldAmount;
            
            currentLanguage = data.language;
            if(currentLanguage == "") currentLanguage = "English";
            ChangeLanguage(currentLanguage);
        }

        [ContextMenu("Save data")]
        private void SaveGameData() {
            SavedData data = new SavedData() {
                goldAmount = goldAmount,
                language = currentLanguage
            };
            save.SaveGameData(data);
        }

        [ContextMenu("Reset data")]
        private void ResetGameData() {
            save.ResetGameData();
            InitData();
        }

        #endregion DATA MANAGER
        
        #region SCENE MANAGER
        /// <summary>
        /// Load new scene with transition
        /// </summary>
        /// <param name="scene"></param>
        public void LoadNewScene(string sceneName) {
            transitionCanvas.DOKill();
            transitionCanvas.DOFade(1, .5f).OnComplete(() => SceneManager.LoadScene(sceneName));
        }
        #endregion SCENE MANAGER
        
        #region LOCALIZATION
        /// <summary>
        /// Change the current language used in the game
        /// </summary>
        /// <param name="language"></param>
        public void ChangeLanguage(string language) {
            LocalizationManager.ChangeLanguage(language);
            currentLanguage = language;
            SaveGameData();
        }
        #endregion Localization

        /// <summary>
        /// Method called when Application Quit
        /// </summary>
        private void OnApplicationQuit() => SaveGameData();
    }

    /// <summary>
    /// Static class that allow to have the sceneNameDirectly
    /// </summary>
    public static class SceneLoader {
        public const string mainMenuScene = "MainMenu";
        public const string inGameScene = "InGameScene";
        public const string victoryScene = "VictoryScene";
    }
}