using UnityEngine;

namespace BL767.DS3
{
    public class CameraHandler : MonoBehaviour
    {
        #region Variables

        [Tooltip("相机跟随的对象，这里指角色")]
        public Transform targetTransform;

        [Tooltip("相机本身")]
        public Transform cameraTransform;

        [Tooltip("相机旋转所绕的轴")]
        public Transform cameraPivotTransform;

        private Transform _myTransform;
        private Vector3 _cameraTransformPos;
        private LayerMask _ignoreLayers;
        private Vector3 _cameraFollowVelocity = Vector3.zero;

        [HideInInspector]
        public static CameraHandler singleton;

        public float lookSpeed = 0.1f;
        public float followSpeed = 0.1f;
        public float pivotSpeed = 0.03f;

        private float _targetPos;
        private float _defaultPos;
        private float _lookAngle;
        private float _pivotAngle;

        [Tooltip("相机向上可旋转的最大角度")]
        public float minPivot = -35;

        [Tooltip("相机向下可旋转的最大角度")]
        public float maxPivot = 35;

        [Tooltip("将相机假设为一个球体，设置它的半径，为碰撞做准备")]
        public float cameraSphereRadius = 0.2f;

        public float cameraCollisionOffset = 0.2f;
        public float minimumCollisionOffset = 0.2f;

        #endregion Variables

        #region MonoBehaviours Callbacks

        private void Awake()
        {
            // using Singleton Design Pattern
            if (singleton == null) singleton = this;
            _myTransform = transform;
            // 实际上这个是相机离人物的水平距离
            _defaultPos = cameraTransform.localPosition.z;
            //
            _ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
        }

        #endregion MonoBehaviours Callbacks

        #region Public Methods

        public void FollowTarget(float delta)
        {
            // targetTransform相当于玩家，让相机跟随玩家
            var targetPos = Vector3.SmoothDamp(
                _myTransform.position,
                targetTransform.position,
                ref _cameraFollowVelocity,
                delta / followSpeed);
            _myTransform.position = targetPos;

            HandleCameraCollision(delta);
        }

        /// <summary>
        /// 处理相机旋转，并将y轴的旋转角度限制
        /// </summary>
        /// <param name="delta"></param>
        /// <param name="mouseXInput"></param>
        /// <param name="mouseYInput"></param>
        public void HandleCameraRotation(float delta, float mouseXInput, float mouseYInput)
        {
            _lookAngle += (mouseXInput * lookSpeed) / delta;
            _pivotAngle -= (mouseYInput * pivotSpeed) / delta;
            _pivotAngle = Mathf.Clamp(_pivotAngle, minPivot, maxPivot);

            var rotation = Vector3.zero;
            // 对摄像机旋转进行Lerp
            // 当鼠标在x轴移动，变换成相机角度旋转则是绕着y轴转，而鼠标在x轴的移动输入即为绕着y轴转的度数
            rotation.y = _lookAngle;
            var targetRotation = Quaternion.Euler(rotation);
            _myTransform.rotation = Quaternion.Lerp(_myTransform.rotation, targetRotation, delta * 10f);

            rotation = Vector3.zero;
            rotation.x = _pivotAngle;

            targetRotation = Quaternion.Euler(rotation);
            cameraPivotTransform.localRotation =
                Quaternion.Lerp(cameraPivotTransform.localRotation, targetRotation, delta * 27f);
        }

        #endregion Public Methods

        #region Private Methods

        private void HandleCameraCollision(float delta)
        {
            _targetPos = _defaultPos;
            var dir = cameraTransform.position - cameraPivotTransform.position;
            dir.Normalize();
            // 射线检测
            var isDetected = Physics.SphereCast(cameraPivotTransform.position,
                                                cameraSphereRadius,
                                                dir,
                                                out var hit,
                                                Mathf.Abs(_targetPos),
                                                _ignoreLayers);
            // 如果像机碰撞，那么相机离人物距离将减少
            if (isDetected)
            {
                var distance = Vector3.Distance(cameraPivotTransform.position, hit.point);
                _targetPos = -(distance - cameraCollisionOffset);
            }
            // 碰撞达到最小允许拉近距离
            if (Mathf.Abs(_targetPos) < minimumCollisionOffset)
            {
                _targetPos = -minimumCollisionOffset;
            }
            // 插值，让摄像机碰撞时更新距离
            _cameraTransformPos.z = Mathf.Lerp(cameraTransform.localPosition.z, _targetPos, delta / 0.2f);
            cameraTransform.localPosition = _cameraTransformPos;
        }

        #endregion Private Methods
    }
}