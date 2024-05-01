using System;
using Helper;
using HubcapManager;
using UnityEngine;

namespace HubcapInterface {
    public class MainMenuManager : Singleton<MainMenuManager> {
        [SerializeField] private UISelectable baseSelectionObj = null;
        private UISelectable lastSelectedObj = null;

        #region BASIC BUTTON METHODS
        /// <summary>
        /// Launch the game
        /// </summary>
        public void LaunchGame() => GameManager.Instance.LoadNewScene(SceneLoader.inGameScene);

        /// <summary>
        /// Exit the app
        /// </summary>
        public void QuitGame() => Application.Quit();
        #endregion BASIC BUTTON METHODS
        
        protected override void AwakeContinue() {
            base.AwakeContinue();
            InputManager.Instance.OnInputChange += SwitchCurrentInput;
            InputManager.Instance.OnInputDirection += SwitchSelectableElementBasedOnDirection;
            InputManager.Instance.OnInteractionKeyPressed += InteractionKeyPressed;
        }

        private void OnDisable() {
            InputManager.Instance.OnInputChange -= SwitchCurrentInput;
            InputManager.Instance.OnInputDirection -= SwitchSelectableElementBasedOnDirection;
            InputManager.Instance.OnInteractionKeyPressed -= InteractionKeyPressed;
        }

        #region METHODS EVENT

        /// <summary>
        /// Method called when the interact key is pressed
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void InteractionKeyPressed(object obj) {
            if (lastSelectedObj != null) lastSelectedObj.ActionInputPressed();
        }

        /// <summary>
        /// Switch between keyboard and gamepad
        /// </summary>
        /// <param name="isKeyboard"></param>
        private void SwitchCurrentInput(object isKeyboard) {
            switch (isKeyboard) {
                case true:
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    if (lastSelectedObj != null) lastSelectedObj.DisableSelection();
                    break;

                case false:
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    if (lastSelectedObj != null) lastSelectedObj.EnableSelection();
                    else baseSelectionObj.EnableSelection();
                    break;
            }
        }

        /// <summary>
        /// Switch the currently selected object based on the direction selected by the player
        /// </summary>
        /// <param name="direction"></param>
        private void SwitchSelectableElementBasedOnDirection(object direction) => lastSelectedObj.TryGoInDirection((Direction) direction);

        #endregion METHODS EVENT

        /// <summary>
        /// Update the currently selected object in the UI
        /// </summary>
        /// <param name="obj"></param>
        public void UpdateSelectedObj(UISelectable obj) => lastSelectedObj = obj;
    }
}
