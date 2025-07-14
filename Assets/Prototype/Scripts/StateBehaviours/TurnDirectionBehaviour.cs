using UnityEngine;

namespace ImmortalSuffering.StateBehaviours
{
    public class TurnDirectionBehaviour : StateMachineBehaviour
    {
        [System.Flags] enum DirMode { Param = 1, Context = 2 }
        enum SrcDirMode { ToggleFromParam, ToggleFromContext }
        // enum SrcDirMode { LookAtTarget, ToggleFromParam, ToggleFromContext }
        [SerializeField] private SrcDirMode srcDirMode;
        [SerializeField] private DirMode dstDirMode;
        [SerializeField] private string paramName;
        [SerializeField] private bool animateRotation = true;
        [SerializeField] private float rightRotation = 180f;
        [SerializeField] private float leftRotation = 0f;
        private EnemyFSMContext context;
        private int paramHash;
        // private bool facingRight;
        // private bool shouldTurn;
        private Vector2 fromTo = new Vector2(0, 180f);

        // public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        // {
        //     base.OnStateMachineEnter(animator, stateMachinePathHash);
        //     context = animator.GetComponent<EnemyFSMContext>();
        //     paramHash = Animator.StringToHash(paramName);
        // }

        private bool initialized = false;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            if (!initialized)
            {
                initialized = true;
                context = animator.GetComponent<EnemyFSMContext>();
                paramHash = Animator.StringToHash(paramName);
            }

            bool facingRight = srcDirMode switch
            {
                SrcDirMode.ToggleFromParam => !animator.GetBool(paramHash),
                _ => !context.FacingRight
            };

            if ((dstDirMode & DirMode.Param) != 0)
                animator.SetBool(paramHash, facingRight);
            if ((dstDirMode & DirMode.Context) != 0)
                context.FacingRight = facingRight;
            fromTo = facingRight ? new Vector2(leftRotation, rightRotation) : new Vector2(rightRotation, leftRotation);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!animateRotation) return; // || !shouldTurn) return;

            animator.transform.rotation = Quaternion.AngleAxis(Mathf.LerpAngle(fromTo.x, fromTo.y, stateInfo.normalizedTime), Vector3.up);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (animateRotation)
                animator.transform.rotation = Quaternion.AngleAxis(fromTo.y, Vector3.up); //shouldTurn ? fromTo.y : (facingRight ? 0f : 180f), Vector3.up);
        }
    }
}