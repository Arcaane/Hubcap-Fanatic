using DG.Tweening;
using Helper;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HubcapManager {
    public class GameManager : Singleton<GameManager> {
        [SerializeField] private SaveDataHandler save = null;
        
        [Header("Saved Data")] 
        [SerializeField] private int goldAmount = 0;
        private SavedData data = null;

        [Header("Scene Transition")] 
        [SerializeField] private CanvasGroup transitionCanvas = null;

        protected override void AwakeContinue() {
            base.AwakeContinue();
            InitData();
            SceneManager.sceneLoaded += (_, _) => {
                transitionCanvas.DOKill();
                transitionCanvas.DOFade(0, 1f);
            };
        }

        /// <summary>
        /// Load data and store them here
        /// </summary>
        private void InitData() {
            data = save.LoadGameData();
            goldAmount = data.goldAmount;
        }

        [ContextMenu("Save data")]
        private void SaveGameData() {
            SavedData data = new SavedData() {
                goldAmount = goldAmount
            };
            save.SaveGameData(data);
        }
        
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
        public const string inGameScene = "GameplayScene";
        public const string victoryScene = "VictoryScene";
    }
}