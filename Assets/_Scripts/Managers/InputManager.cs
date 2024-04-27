using Helper;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager> {
    private PlayerInputs inputs = null;
    private bool isKeyboard = true;

    public delegate void InputActionEvent(object obj);
    public event InputActionEvent OnInputChange;
    public event InputActionEvent OnInputDirection;
    public event InputActionEvent OnInteractionKeyPressed;
    public event InputActionEvent OnRightStickMove;

    protected override void AwakeContinue() {
        base.AwakeContinue();
        inputs = new PlayerInputs();
        inputs.Enable();

        InputSystem.onActionChange += InputActionChangeCallback;
        inputs.Menu.Move.started += TryToMoveInMenu;
        inputs.Menu.Interaction.started += (_) => {
            if(!isKeyboard) OnInteractionKeyPressed?.Invoke(null);
        };
        inputs.Menu.CameraRotation.performed += (_) => {
            if(!isKeyboard) OnRightStickMove?.Invoke(inputs.Menu.CameraRotation.ReadValue<Vector2>());
        };
    }

    /// <summary>
    /// Method called when direction is pressed (arrow or stick on any gamepad)
    /// </summary>
    /// <param name="obj"></param>
    private void TryToMoveInMenu(InputAction.CallbackContext obj) {
        float DoEast = Vector2.Dot(Vector2.right, obj.ReadValue<Vector2>());
        float DoNorth = Vector2.Dot(Vector2.up, obj.ReadValue<Vector2>());
        if (!isKeyboard) OnInputDirection?.Invoke(Mathf.Abs(DoEast) > Mathf.Abs(DoNorth) ? (DoEast < 0 ? Direction.West : Direction.East) : (DoNorth < 0 ? Direction.South : Direction.North));
    }

    /// <summary>
    /// Know which controller is currently use
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="change"></param>
    private void InputActionChangeCallback(object obj, InputActionChange change) {
        //isKeyboardAndMouse = lastDevice.name.Equals("Keyboard") || lastDevice.name.Equals("Mouse");
        //If needed we could check for "XInputControllerWindows" or "DualShock4GamepadHID"
        //Maybe if it Contains "controller" could be xbox layout and "gamepad" sony? More investigation needed
        if (obj is not InputAction action) return;
        if (action.activeControl == null) return;
        InputDevice lastDevice = action.activeControl.device;

        bool isLastInputKeyboard = lastDevice.name.Equals("Keyboard") || lastDevice.name.Equals("Mouse");
        if (isKeyboard != isLastInputKeyboard) {
            isKeyboard = isLastInputKeyboard;
            OnInputChange?.Invoke(isKeyboard);
        }
        //Debug.Log(lastDevice.name);
    }

    #region INPUT STATE
    public void EnableInputs() => inputs.Enable();
    public void DisableInputs() => inputs.Disable();
    #endregion INPUT STATE
}

public enum Direction {
    North,
    South,
    East,
    West
}