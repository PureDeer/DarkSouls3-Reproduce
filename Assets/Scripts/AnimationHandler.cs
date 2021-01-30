using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BL767.DS3
{
    public class AnimationHandler : MonoBehaviour
    {
        public Animator anim;
        public bool canRotate;

        private int vertical;
        private int horizontal;

        public void Initialize()
        {
            anim = GetComponent<Animator>();
            vertical = Animator.StringToHash("Vertical");
            horizontal = Animator.StringToHash("Horizontal");
        }

        public void UpdateAnimatorValues(float verticalMovement, float horizontaiMovement)
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

            // damp float, 动画过渡
            anim.SetFloat(vertical, v, 0.1f, Time.deltaTime);
            anim.SetFloat(horizontal, h, 0.1f, Time.deltaTime);
        }

        public void CanRotate() => canRotate = true;

        public void StopRotation() => canRotate = false;
    }
}