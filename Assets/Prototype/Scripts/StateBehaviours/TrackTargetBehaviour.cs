using UnityEngine;

namespace ImmortalSuffering.StateBehaviours
{
    /// <summary>
    /// context.Target과 animator.transform의 위치를 추적하며 x좌표 반전 시 지정한 Trigger 발생.
    /// </summary>
    public class TrackTargetBehaviour : StateMachineBehaviour
    {
        [SerializeField] private string triggerName;
        private EnemyFSMContext context;
        private int paramHash;
        private bool wasTargetOnRight;

        // public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        // {
        //     context = animator.GetComponent<EnemyFSMContext>();
        //     paramHash = Animator.StringToHash(triggerName);
        // }

        private bool initialized = false;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!initialized)
            {
                initialized = true;
                context = animator.GetComponent<EnemyFSMContext>();
                paramHash = Animator.StringToHash(triggerName);
            }
            wasTargetOnRight = context.Target.position.x > animator.transform.position.x;
            if (wasTargetOnRight ^ context.FacingRight)
                animator.SetTrigger(paramHash);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (wasTargetOnRight ^ (context.Target.position.x > animator.transform.position.x))
            {
                animator.SetTrigger(paramHash);
                wasTargetOnRight ^= true;
            }
        }
    }
}