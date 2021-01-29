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

        public void UpdateAnimatorValues(float p_verticalMovement, float p_horizontaiMovement)
        {
            #region Vertical

            float t_v;

            if (p_verticalMovement > 0 && p_verticalMovement < 0.55f) t_v = 0.5f;
            else if (p_verticalMovement > 0.55f) t_v = 1;
            else if (p_verticalMovement < 0 && p_verticalMovement > -0.55f) t_v = -0.5f;
            else if (p_verticalMovement < -0.55f) t_v = -1;
            else t_v = 0;

            #endregion Vertical

            #region Horizontal

            float t_h;

            if (p_horizontaiMovement > 0 && p_horizontaiMovement < 0.55f) t_h = 0.5f;
            else if (p_horizontaiMovement > 0.55f) t_h = 1;
            else if (p_horizontaiMovement < 0 && p_horizontaiMovement > -0.55f) t_h = -0.5f;
            else if (p_horizontaiMovement < -0.55f) t_h = -1;
            else t_h = 0;

            #endregion Horizontal

            // damp float, 动画过渡
            anim.SetFloat(vertical, t_v, 0.1f, Time.deltaTime);
            anim.SetFloat(horizontal, t_h, 0.1f, Time.deltaTime);
        }

        public void CanRotate() => canRotate = true;

        public void StopRotation() => canRotate = false;
    }
}