using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BL767.DS3
{
    public class CameraHandler : MonoBehaviour
    {
        #region Variables

        [Tooltip("The camera will eventually go to")]
        public Transform targetTransform;

        [Tooltip("The actual camera")]
        public Transform cameraTransform;

        [Tooltip("The pivot transform that camera rotate around")]
        public Transform cameraPivotTransform;

        private Transform myTransform;
        private Vector3 cameraTransformPos;
        private LayerMask ignoreLayers;

        public static CameraHandler singleton;

        public float lookSpeed = 0.1f;
        public float followSpeed = 0.1f;
        public float pivotSpeed = 0.03f;

        private float defaultPos;
        private float lookAngle;
        private float pivotAngle;

        [Tooltip("相机向上可旋转的最大角度")]
        public float minPivot = -35;

        [Tooltip("相机向下可旋转的最大角度")]
        public float maxPivot = 35;

        #endregion Variables

        #region MonoBehaviours Callbacks

        private void Awake()
        {
            // using Singleton Design Pattern
            if (singleton == null) singleton = this;
            myTransform = transform;
            defaultPos = cameraTransform.localPosition.z;
            ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
        }

        #endregion MonoBehaviours Callbacks

        #region Public Methods

        public void FollowTarget(float delta)
        {
            // targetTransform相当于玩家，让相机跟随玩家
            Vector3 targetPos = Vector3.Lerp(myTransform.position, targetTransform.position, delta / followSpeed);
            myTransform.position = targetPos;
        }

        /// <summary>
        /// 处理相机旋转，并将y轴的旋转角度限制
        /// </summary>
        /// <param name="delta"></param>
        /// <param name="mouseXInput"></param>
        /// <param name="mouseYInput"></param>
        public void HandleCameraRotation(float delta, float mouseXInput, float mouseYInput)
        {
            lookAngle += (mouseXInput * lookSpeed) / delta;
            pivotAngle -= (mouseYInput * pivotSpeed) / delta;
            pivotAngle = Mathf.Clamp(pivotAngle, minPivot, maxPivot);

            Vector3 rotation = Vector3.zero;
            // 当鼠标在x轴移动，变换成相机角度旋转则是绕着y轴转，而鼠标在x轴的移动输入则为绕着y轴转的度数
            rotation.y = lookAngle;
            Quaternion targetRotation = Quaternion.Euler(rotation);
            myTransform.rotation = targetRotation;

            rotation = Vector3.zero;
            rotation.x = pivotAngle;

            targetRotation = Quaternion.Euler(rotation);
            cameraPivotTransform.localRotation = targetRotation;
        }

        #endregion Public Methods
    }
}