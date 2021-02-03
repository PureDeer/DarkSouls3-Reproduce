using UnityEngine;

namespace BL767.DS3
{
    public class PlayerLocomotion : MonoBehaviour
    {
        #region Variables

        private PlayerManager _pm;
        private Transform _cameraObject;
        private InputHandler _inputHandler;
        private Vector3 _moveDirection;

        [Header("Movement Stats")]
        [SerializeField]
        private float _movementSpeed = 4.0f;

        [SerializeField]
        private float _sprintSpeed = 7.0f;

        [SerializeField]
        private float _rotationSpeed = 10.0f;

        private Vector3 _normalVector;
        private Vector3 _targetPosition;

        [Tooltip("from UnityEngine.Attributes")]
        [HideInInspector]
        public Transform myTransform;

        [HideInInspector]
        public AnimationHandler animationHandler;

        public new Rigidbody rigidbody;
        public GameObject normalCamera;

        #endregion Variables

        #region MonoBehaviour Callbacks

        private void Start()
        {
            _pm = GetComponent<PlayerManager>();
            rigidbody = GetComponent<Rigidbody>();
            _inputHandler = GetComponent<InputHandler>();
            // 因为AnimationHandler挂载在模型上，它在子层级
            animationHandler = GetComponentInChildren<AnimationHandler>();
            _cameraObject = Camera.main.transform;
            myTransform = transform;

            animationHandler.Initialize();
        }

        #endregion MonoBehaviour Callbacks

        #region Movements

        /// <summary>
        /// 负责人物移动矢量
        /// </summary>
        /// <param name="delta"></param>
        public void HandleMovement(float delta)
        {
            // 如果在交互中，不允许做其他动作
            if (_inputHandler.rollFlag) return;

            _moveDirection = _cameraObject.forward * _inputHandler.vertical;
            _moveDirection += _cameraObject.right * _inputHandler.horizontal;
            _moveDirection.Normalize();
            // 因为_moveDirection是随着_cameraObject变换的，当鼠标输入时，_cameraObject是会上下移动的，
            // 所以需要在这里将_moveDirection的y轴矢量置为0
            _moveDirection.y = 0;

            // 奔跑，运动方向就乘以奔跑的长度
            // 否则，乘以普通行走的长度
            if (_inputHandler.sprintFlag)
            {
                _pm.isSprinting = true;
                _moveDirection *= _sprintSpeed;
            }
            else
            {
                // 将速度加入到move
                _moveDirection *= _movementSpeed;
            }

            // _moveDirection会随着输入一直更新，所以会一直把_moveDirection投射到plane上
            // 再将投射后的矢量赋给刚体的速度，即达成move
            var projectedVelocity = Vector3.ProjectOnPlane(_moveDirection, _normalVector);
            rigidbody.velocity = projectedVelocity;

            animationHandler.UpdateAnimatorValues(_inputHandler.moveAmount, 0, _pm.isSprinting);

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
            if (_inputHandler.rollFlag)
            {
                _moveDirection = _cameraObject.forward * _inputHandler.vertical;
                _moveDirection += _cameraObject.right * _inputHandler.horizontal;
                // 如果有动作，执行Rolling
                if (_inputHandler.moveAmount > 0)
                {
                    // 播放指定动画，并把isInteracting设置为true
                    animationHandler.PlayTargetAnimation("Rolling", true);
                    _moveDirection.y = 0;

                    Quaternion rollRotation = Quaternion.LookRotation(_moveDirection);
                    myTransform.rotation = rollRotation;
                }
                else
                {
                    animationHandler.PlayTargetAnimation("Backstep", true);
                }
            }
        }

        /// <summary>
        /// 处理人物旋转
        /// </summary>
        /// <param name="delta">插值的过渡时间参数</param>
        private void HandleRotation(float delta)
        {
            // 初始化目标方向
            var targetDir = Vector3.zero;
            // 矢量加和结果
            var moveOverride = _inputHandler.moveAmount;
            // 输入方向
            targetDir += _cameraObject.forward * _inputHandler.vertical;
            targetDir += _cameraObject.right * _inputHandler.horizontal;
            // 归一化，并将y轴矢量置为0
            targetDir.Normalize();
            targetDir.y = 0;

            // 如果没有位移，则将方向置为前方
            if (targetDir == Vector3.zero) targetDir = myTransform.forward;

            var rs = _rotationSpeed;
            // 返回一个转向给定方向之后的角度
            var tr = Quaternion.LookRotation(targetDir);
            // 从原始角度插值到给定角度
            myTransform.rotation = Quaternion.Slerp(myTransform.rotation, tr, rs * delta);
        }

        #endregion Movements
    }
}