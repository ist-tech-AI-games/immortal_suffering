using UnityEngine;

namespace ImmortalSuffering.StateBehaviours
{
    public class DestroyOnExitBehaviour : StateMachineBehaviour
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Destroy(animator.gameObject);
        }
    }
}