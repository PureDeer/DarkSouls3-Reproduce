using UnityEngine;

namespace BL767.DS3
{
    public class InputHandler : MonoBehaviour
    {
        #region Variables

        public float horizontal;
        public float vertical;
        public float moveAmount;
        public float mouseX;
        public float mouseY;

        public bool bInput;
        public bool rollFlag;
        public bool sprintFlag;

        public float rollInputTimer;

        private PlayerControls _inputActions;

        private Vector2 _movementInput;
        private Vector2 _cameraInput;

        #endregion Variables

        #region MonoBehaviour Callbacks

        private void OnEnable()
        {
            if (_inputActions == null)
            {
                _inputActions = new PlayerControls();
                // lambda表达式 添加事件。读取键盘输入和鼠标输入
                _inputActions.PlayerMovements.Movements.performed += inputActions => _movementInput = inputActions.ReadValue<Vector2>();
                _inputActions.PlayerMovements.Camera.performed += inputActions => _cameraInput = inputActions.ReadValue<Vector2>();
            }

            _inputActions.Enable();
        }

        private void OnDisable()
        {
            _inputActions.Disable();
        }

        #endregion MonoBehaviour Callbacks

        #region Public Methods

        public void TickInput(float delta)
        {
            MoveInput(delta);
            HandleRollInput(delta);
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// 角色动作输入，包括键盘和鼠标
        /// </summary>
        /// <param name="delta"></param>
        private void MoveInput(float delta)
        {
            horizontal = _movementInput.x;
            vertical = _movementInput.y;
            // 向量加和
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));

            mouseX = _cameraInput.x;
            mouseY = _cameraInput.y;
        }

        /// <summary>
        /// 负责角色前滚输入
        /// </summary>
        /// <param name="delta"></param>
        private void HandleRollInput(float delta)
        {
            // 检测是否有按键输入
            bInput = _inputActions.PlayerActions.Roll.phase == UnityEngine.InputSystem.InputActionPhase.Started;

            if (bInput)
            {
                rollInputTimer += delta;
                sprintFlag = true;
            }
            else
            {
                if (rollInputTimer > 0 && rollInputTimer < 0.5f)
                {
                    sprintFlag = false;
                    rollFlag = true;
                }

                rollInputTimer = 0;
            }
        }

        #endregion Private Methods
    }
}