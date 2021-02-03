using UnityEngine;

namespace BL767.DS3
{
    public class ResetIsInteracting : StateMachineBehaviour
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // 推出动画时，设isInteracting为False;
            animator.SetBool("isInteracting", false);
        }
    }
}