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
        private float rotationSpeed = 10.0f;

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
            float t_delta = Time.deltaTime;
            // 更新输入
            inputHandler.TickInput(t_delta);
            moveDirection = cameraObject.forward * inputHandler.vertical;
            moveDirection += cameraObject.right * inputHandler.horizontal;
            moveDirection.Normalize();
            // 将速度加入到move
            float t_speed = movementSpeed;
            moveDirection *= t_speed;
            // moveDirection会随着输入一直更新，所以会一直把moveDirection投射到plane上
            // 再将投射后的矢量赋给刚体的速度，即达成move
            Vector3 t_projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            rigidbody.velocity = t_projectedVelocity;

            animationHandler.UpdateAnimatorValues(inputHandler.moveAmount, 0);

            // 旋转
            if (animationHandler.canRotate) HandleRotation(t_delta);
        }

        #endregion MonoBehaviour Callbacks

        #region Movements

        private Vector3 normalVector;
        private Vector3 targetPosition;

        /// <summary>
        /// 处理人物旋转
        /// </summary>
        /// <param name="p_delta">插值的过渡事件参数</param>
        private void HandleRotation(float p_delta)
        {
            // 初始化目标方向
            Vector3 t_targetDir = Vector3.zero;
            // 矢量加和结果
            float t_moveOverride = inputHandler.moveAmount;
            // 输入方向
            t_targetDir += cameraObject.forward * inputHandler.vertical;
            t_targetDir += cameraObject.right * inputHandler.horizontal;
            // 归一化，并将y轴矢量置为0
            t_targetDir.Normalize();
            t_targetDir.y = 0;
            // 如果没有位移，则将方向置为前方
            if (t_targetDir == Vector3.zero) t_targetDir = myTransform.forward;

            float t_rs = rotationSpeed;
            // 返回一个转向给定方向之后的角度
            Quaternion t_tr = Quaternion.LookRotation(t_targetDir);
            // 从原始角度插值到给定角度
            myTransform.rotation = Quaternion.Slerp(myTransform.rotation, t_tr, t_rs * p_delta);
        }

        #endregion Movements
    }
}