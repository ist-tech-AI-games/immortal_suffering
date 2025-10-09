using UnityEngine;

namespace ImmortalSuffering.StateBehaviours
{
    public class TurnDirectionBehaviour : StateMachineBehaviour
    {
        [System.Flags] enum DirMode { Param = 1, Context = 2 }
        enum SrcDirMode { ToggleFromParam, ToggleFromContext }
        enum RotationControlMode { DontUseRotation, Animate, SetOnEnter, SetOnExit }
        // enum SrcDirMode { LookAtTarget, ToggleFromParam, ToggleFromContext }
        [SerializeField] private SrcDirMode srcDirMode;
        [SerializeField] private DirMode dstDirMode;
        [SerializeField] private string paramName;
        [SerializeField] private RotationControlMode rotationControlMode = RotationControlMode.Animate;
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

            if (rotationControlMode == RotationControlMode.SetOnEnter)
                SetAngle(animator, fromTo.y);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (rotationControlMode == RotationControlMode.Animate)
                SetAngle(animator, Mathf.LerpAngle(fromTo.x, fromTo.y, stateInfo.normalizedTime));
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (rotationControlMode == RotationControlMode.Animate || rotationControlMode == RotationControlMode.SetOnExit)
                SetAngle(animator, fromTo.y);
        }

        private void SetAngle(Animator animator, float angle) { animator.transform.rotation = Quaternion.AngleAxis(angle, Vector3.up); }
    }
}