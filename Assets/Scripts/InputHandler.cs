using UnityEngine;

namespace Mechadroids {
    /// <summary>
    /// Handles input state from the Input System
    /// </summary>
    public class InputHandler {
        private InputActions inputActions;

        public Vector2 MovementInput { get; private set; }
        public Vector2 MouseDelta { get; private set; }
        //public InputActions InputActions => inputActions;

        public InputActions InputActions { get { inputActions ??= new InputActions(); return inputActions; } }

        public void Initialize() {
            // initialize input here
            //inputActions = new InputActions();
            InputActions.Enable();

            InputActions.Player.Move.performed += ctx => MovementInput = ctx.ReadValue<Vector2>();
            InputActions.Player.Move.canceled += ctx => MovementInput = Vector2.zero;


            InputActions.Player.Look.performed += ctx => MouseDelta = ctx.ReadValue<Vector2>();
            InputActions.Player.Look.canceled += ctx => MouseDelta = Vector2.zero;

        }








        public void SetCursorState(bool visibility, CursorLockMode lockMode) {
            Cursor.visible = visibility;
            Cursor.lockState = lockMode;
        }

        public void Dispose() {
            SetCursorState(true, CursorLockMode.None);
            inputActions.Disable();
        }
    }
}
