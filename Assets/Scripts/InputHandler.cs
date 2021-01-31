using System.Collections;
using System.Collections.Generic;
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

        public bool b_Input;
        public bool rollFlag;
        public bool sprintFlag;
        public bool isInteracting;
        public float rollInputTimer;

        private PlayerControls inputActions;
        private CameraHandler cameraHandler;

        private Vector2 movementInput;
        private Vector2 cameraInput;

        #endregion Variables

        #region MonoBehaviour Callbacks

        private void Awake()
        {
            cameraHandler = CameraHandler.singleton;
        }

        private void OnEnable()
        {
            if (inputActions == null)
            {
                inputActions = new PlayerControls();
                // lambda表达式 添加事件。读取键盘输入和鼠标输入
                inputActions.PlayerMovements.Movements.performed += inputActions => movementInput = inputActions.ReadValue<Vector2>();
                inputActions.PlayerMovements.Camera.performed += inputActions => cameraInput = inputActions.ReadValue<Vector2>();
            }

            inputActions.Enable();
        }

        private void OnDisable()
        {
            inputActions.Disable();
        }

        private void FixedUpdate()
        {
            float delta = Time.fixedDeltaTime;

            if (cameraHandler != null)
            {
                cameraHandler.FollowTarget(delta);
                cameraHandler.HandleCameraRotation(delta, mouseX, mouseY);
            }
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
            horizontal = movementInput.x;
            vertical = movementInput.y;
            // 向量加和
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));

            mouseX = cameraInput.x;
            mouseY = cameraInput.y;
        }

        /// <summary>
        /// 负责角色前滚输入
        /// </summary>
        /// <param name="delta"></param>
        private void HandleRollInput(float delta)
        {
            // 检测是否有按键输入
            b_Input = inputActions.PlayerActions.Roll.phase == UnityEngine.InputSystem.InputActionPhase.Started;

            if (b_Input)
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