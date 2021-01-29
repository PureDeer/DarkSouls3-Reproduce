﻿using System.Collections;
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

        private PlayerControls inputActions;

        private Vector2 movementInput;
        private Vector2 cameraInput;

        #endregion Variables

        #region MonoBehaviour Callbacks

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

        #endregion MonoBehaviour Callbacks

        #region Public Methods

        public void TickInput(float t_delta)
        {
            MoveInput(t_delta);
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// 角色动作输入，包括键盘和鼠标
        /// </summary>
        /// <param name="delta"></param>
        private void MoveInput(float t_delta)
        {
            horizontal = movementInput.x;
            vertical = movementInput.y;
            // 向量加和
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));

            mouseX = cameraInput.x;
            mouseY = cameraInput.y;
        }

        #endregion Private Methods
    }
}