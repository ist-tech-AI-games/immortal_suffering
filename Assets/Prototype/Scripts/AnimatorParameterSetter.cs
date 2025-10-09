using UnityEngine;

namespace ImmortalSuffering
{
    public class AnimatorParameterSetter : MonoBehaviour
    {
        [SerializeField] private string paramName;
        private int paramHash;
        private Animator animator;

        void Awake()
        {
            animator = GetComponentInParent<Animator>();
            paramHash = Animator.StringToHash(paramName);
        }

        public void SetBool(bool value) => animator.SetBool(paramHash, value);
        public void SetInteger(int value) => animator.SetInteger(paramHash, value);
        public void SetFloat(float value) => animator.SetFloat(paramHash, value);
        public void SetTrigger() => animator.SetTrigger(paramHash);
    }
}