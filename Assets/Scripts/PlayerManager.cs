using UnityEngine;

namespace BL767.DS3
{
    public class PlayerManager : MonoBehaviour
    {
        private InputHandler _inputHandler;
        private Animator _anim;
        private CameraHandler _cameraHandler;
        private PlayerLocomotion _pLoco;

        [Header("Player Flags")]
        public bool isInteracting;

        public bool isSprinting;

        private void Awake()
        {
            _cameraHandler = CameraHandler.singleton;
        }

        private void Start()
        {
            _inputHandler = GetComponent<InputHandler>();
            _anim = GetComponentInChildren<Animator>();
            _pLoco = GetComponent<PlayerLocomotion>();
        }

        private void Update()
        {
            var delta = Time.deltaTime;

            isInteracting = _anim.GetBool("isInteracting");

            _inputHandler.TickInput(delta);
            _pLoco.HandleMovement(delta);
            _pLoco.HandleRollingAndSprinting(delta);
        }

        private void FixedUpdate()
        {
            var delta = Time.fixedDeltaTime;

            if (_cameraHandler != null)
            {
                _cameraHandler.FollowTarget(delta);
                _cameraHandler.HandleCameraRotation(delta, _inputHandler.mouseX, _inputHandler.mouseY);
            }
        }

        private void LateUpdate()
        {
            // 重置flag
            _inputHandler.rollFlag = false;
            _inputHandler.sprintFlag = false;

            // 更新输入，包括鼠标，键盘
            isSprinting = _inputHandler.bInput;
        }
    }
}