using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BL767.DS3
{
    public class PlayerManager : MonoBehaviour
    {
        private InputHandler inputHandler;
        private Animator anim;

        private void Start()
        {
            inputHandler = GetComponent<InputHandler>();
            anim = GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            inputHandler.isInteracting = anim.GetBool("isInteracting");
            // 重置flag
            inputHandler.rollFlag = false;
            inputHandler.sprintFlag = false;
        }
    }
}