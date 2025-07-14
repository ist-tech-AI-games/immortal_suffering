using UnityEngine;

namespace ImmortalSuffering.StateBehaviours
{
    public class MovementBehaviour : StateMachineBehaviour
    {
        enum DirMode { FromParam, FromContext, FromTarget }
        [SerializeField] private DirMode dirMode;
        [SerializeField] private string dirParamName;
        [SerializeField] private float speed;
        private Rigidbody2D rb2d;
        private EnemyFSMContext context;
        private int paramHash;
        private bool initialized = false;

        // public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (initialized) return;
            initialized = false;
            rb2d = animator.GetComponent<Rigidbody2D>();
            context = animator.GetComponent<EnemyFSMContext>();
            paramHash = Animator.StringToHash(dirParamName);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);
            bool right = dirMode switch
            {
                DirMode.FromParam => animator.GetBool(paramHash),
                DirMode.FromTarget => animator.transform.position.x < context.Target.position.x,
                _ => context.FacingRight,
            };

            rb2d.MovePosition(rb2d.position + (right ? Vector2.right : Vector2.left) * (speed * Time.fixedDeltaTime));
        }
    }
}
