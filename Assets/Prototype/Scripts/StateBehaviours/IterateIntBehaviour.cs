using UnityEngine;

namespace ImmortalSuffering.StateBehaviours
{
    public class IterateIntBehaviour : StateMachineBehaviour
    {
        [System.Flags]
        public enum InvokeEventType
        {
            Enter = 1,
            Exit = 2,
        }

        [SerializeField]
        private InvokeEventType invokeEventType;

        [SerializeField]
        private string targetParam;

        [SerializeField]
        private int[] values;

        [SerializeField]
        private int startIndex = 0;
        private int paramId = 0;
        private int index;

        void OnEnable()
        {
            paramId = Animator.StringToHash(targetParam);
            index = Mathf.Min(startIndex, values.Length - 1);
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if ((invokeEventType & InvokeEventType.Enter) == 0)
                return;
            animator.SetInteger(paramId, values[index]);
            index = (index + 1) % values.Length;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if ((invokeEventType & InvokeEventType.Exit) == 0)
                return;
            animator.SetInteger(paramId, values[index]);
            index = (index + 1) % values.Length;
        }
    }
}
