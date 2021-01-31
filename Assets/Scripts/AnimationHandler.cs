﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BL767.DS3
{
    public class AnimationHandler : MonoBehaviour
    {
        public Animator anim;
        public InputHandler inputHandler;
        public PlayerLocomotion plm;
        public bool canRotate;

        private int vertical;
        private int horizontal;

        public void Initialize()
        {
            anim = GetComponent<Animator>();
            plm = GetComponentInParent<PlayerLocomotion>();
            inputHandler = GetComponentInParent<InputHandler>();
            vertical = Animator.StringToHash("Vertical");
            horizontal = Animator.StringToHash("Horizontal");
        }

        public void UpdateAnimatorValues(float verticalMovement, float horizontaiMovement, bool isSprinting)
        {
            #region Vertical

            float v;

            if (verticalMovement > 0 && verticalMovement < 0.55f) v = 0.5f;
            else if (verticalMovement > 0.55f) v = 1;
            else if (verticalMovement < 0 && verticalMovement > -0.55f) v = -0.5f;
            else if (verticalMovement < -0.55f) v = -1;
            else v = 0;

            #endregion Vertical

            #region Horizontal

            float h;

            if (horizontaiMovement > 0 && horizontaiMovement < 0.55f) h = 0.5f;
            else if (horizontaiMovement > 0.55f) h = 1;
            else if (horizontaiMovement < 0 && horizontaiMovement > -0.55f) h = -0.5f;
            else if (horizontaiMovement < -0.55f) h = -1;
            else h = 0;

            #endregion Horizontal

            // 奔跑
            if (isSprinting)
            {
                v = 2;
                h = horizontaiMovement;
            }

            // damp float, 动画过渡
            anim.SetFloat(vertical, v, 0.1f, Time.deltaTime);
            anim.SetFloat(horizontal, h, 0.1f, Time.deltaTime);
        }

        public void PlaytargetAnimation(string targetAnim, bool isInteracting)
        {
            anim.applyRootMotion = isInteracting;
            anim.SetBool("isInteracting", isInteracting);
            // 使用标准化时间创建从当前状态到任何其他状态的交叉淡入淡出。
            anim.CrossFade(targetAnim, 0.2f);
        }

        public void CanRotate() => canRotate = true;

        public void StopRotation() => canRotate = false;

        private void OnAnimatorMove()
        {
            if (!inputHandler.isInteracting) return;

            float delta = Time.deltaTime;
            plm.rigidbody.drag = 0;
            Vector3 deltaPos = anim.deltaPosition;
            deltaPos.y = 0;
            Vector3 velocity = deltaPos / delta;
            plm.rigidbody.velocity = velocity;
        }
    }
}