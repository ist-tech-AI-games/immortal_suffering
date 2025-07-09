using UnityEngine;

// A behaviour that is attached to a playable
namespace ImmortalSuffering.StateBehaviours
{
    public class ToggleFlagBehaviour : StateMachineBehaviour
    {
        [SerializeField] private string targetParam;
        private int paramId;

        void OnEnable()
        {
            paramId = Animator.StringToHash(targetParam);
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            bool cur = animator.GetBool(paramId);
            animator.SetBool(paramId, !cur);
        }
    }
}
