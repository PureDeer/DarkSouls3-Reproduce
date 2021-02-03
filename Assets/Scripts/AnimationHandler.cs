using UnityEngine;

namespace BL767.DS3
{
    public class AnimationHandler : MonoBehaviour
    {
        private PlayerManager _pm;
        public Animator anim;
        private InputHandler _inputHandler;
        private PlayerLocomotion _plm;
        public bool canRotate;

        private int _vertical;
        private int _horizontal;

        public void Initialize()
        {
            _pm = GetComponentInParent<PlayerManager>();
            anim = GetComponent<Animator>();
            _plm = GetComponentInParent<PlayerLocomotion>();
            _inputHandler = GetComponentInParent<InputHandler>();
            _vertical = Animator.StringToHash("Vertical");
            _horizontal = Animator.StringToHash("Horizontal");
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

            // 奔跑，并且解决住原地后撤时会先显示跑步的动画的Bug
            if (isSprinting && !Mathf.Approximately(_inputHandler.moveAmount, 0))
            {
                v = 2;
                h = horizontaiMovement;
            }

            // damp float, 动画过渡
            anim.SetFloat(_vertical, v, 0.1f, Time.deltaTime);
            anim.SetFloat(_horizontal, h, 0.1f, Time.deltaTime);
        }

        public void PlayTargetAnimation(string targetAnim, bool isInteracting)
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
            if (!_pm.isInteracting) return;

            var delta = Time.deltaTime;
            _plm.rigidbody.drag = 0;
            var deltaPos = anim.deltaPosition;
            deltaPos.y = 0;
            var velocity = deltaPos / delta;
            _plm.rigidbody.velocity = velocity;
        }
    }
}