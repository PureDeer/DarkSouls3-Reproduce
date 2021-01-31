using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BL767.DS3
{
    public class PlayerLocomotion : MonoBehaviour
    {
        #region Variables

        private Transform cameraObject;
        private InputHandler inputHandler;
        private Vector3 moveDirection;

        // from UnityEngine.Attributes
        [HideInInspector]
        public Transform myTransform;

        [HideInInspector]
        public AnimationHandler animationHandler;

        public new Rigidbody rigidbody;
        public GameObject normalCamera;

        [Header("Stats")]
        [SerializeField]
        private float movementSpeed = 5.0f;

        [SerializeField]
        private float sprintSpeed = 7.0f;

        [SerializeField]
        private float rotationSpeed = 10.0f;

        public bool isSprinting;

        #endregion Variables

        #region MonoBehaviour Callbacks

        private void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            inputHandler = GetComponent<InputHandler>();
            // 因为AnimationHandler挂载在模型上，它在子层级
            animationHandler = GetComponentInChildren<AnimationHandler>();
            cameraObject = Camera.main.transform;
            myTransform = transform;

            animationHandler.Initialize();
        }

        private void Update()
        {
            float delta = Time.deltaTime;
            // 更新输入，包括鼠标，键盘
            isSprinting = inputHandler.b_Input;
            inputHandler.TickInput(delta);
            HandleMovement(delta);
            HandleRollingAndSprinting(delta);
        }

        #endregion MonoBehaviour Callbacks

        #region Movements

        public void HandleMovement(float delta)
        {
            // 如果在交互中，不允许做其他动作
            if (inputHandler.rollFlag) return;

            moveDirection = cameraObject.forward * inputHandler.vertical;
            moveDirection += cameraObject.right * inputHandler.horizontal;
            moveDirection.Normalize();
            // 因为moveDirection是随着cameraObject变换的，当鼠标输入时，cameraObject是会上下移动的，
            // 所以需要在这里将moveDirection的y轴矢量置为0
            moveDirection.y = 0;

            // 奔跑
            if (inputHandler.sprintFlag)
            {
                movementSpeed = sprintSpeed;
                isSprinting = true;
                moveDirection *= movementSpeed;
            }
            else
            {
                // 将速度加入到move
                moveDirection *= movementSpeed;
            }

            // moveDirection会随着输入一直更新，所以会一直把moveDirection投射到plane上
            // 再将投射后的矢量赋给刚体的速度，即达成move
            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            rigidbody.velocity = projectedVelocity;

            animationHandler.UpdateAnimatorValues(inputHandler.moveAmount, 0, isSprinting);

            // 旋转
            if (animationHandler.canRotate) HandleRotation(delta);
        }

        /// <summary>
        /// 负责处理翻滚和奔跑。
        /// </summary>
        /// <param name="delta"></param>
        public void HandleRollingAndSprinting(float delta)
        {
            if (animationHandler.anim.GetBool("isInteracting")) return;

            // 如果有翻滚键输入
            if (inputHandler.rollFlag)
            {
                moveDirection = cameraObject.forward * inputHandler.vertical;
                moveDirection += cameraObject.right * inputHandler.horizontal;
                // 如果有动作，执行Rolling
                if (inputHandler.moveAmount > 0)
                {
                    // 播放指定动画，并把isInteracting设置为true
                    animationHandler.PlaytargetAnimation("Rolling", true);
                    moveDirection.y = 0;

                    Quaternion rollRotation = Quaternion.LookRotation(moveDirection);
                    myTransform.rotation = rollRotation;
                }
                else
                {
                    animationHandler.PlaytargetAnimation("Backstep", true);
                }
            }
        }

        private Vector3 normalVector;
        private Vector3 targetPosition;

        /// <summary>
        /// 处理人物旋转
        /// </summary>
        /// <param name="delta">插值的过渡时间参数</param>
        private void HandleRotation(float delta)
        {
            // 初始化目标方向
            Vector3 targetDir = Vector3.zero;
            // 矢量加和结果
            float t_moveOverride = inputHandler.moveAmount;
            // 输入方向
            targetDir += cameraObject.forward * inputHandler.vertical;
            targetDir += cameraObject.right * inputHandler.horizontal;
            // 归一化，并将y轴矢量置为0
            targetDir.Normalize();
            targetDir.y = 0;

            // 如果没有位移，则将方向置为前方
            if (targetDir == Vector3.zero) targetDir = myTransform.forward;

            float rs = rotationSpeed;
            // 返回一个转向给定方向之后的角度
            Quaternion tr = Quaternion.LookRotation(targetDir);
            // 从原始角度插值到给定角度
            myTransform.rotation = Quaternion.Slerp(myTransform.rotation, tr, rs * delta);
        }

        #endregion Movements
    }
}