using UnityEngine;

namespace ImmortalSuffering
{
    /// <summary>
    /// StateMachineBehaviour의 외부 Scene 종속 컴포넌트 참조 문제를 해결하기 위한 중간 wrapper.
    /// 해당 Behaviour를 사용하는 Animator와 같은 위치에 배치할 것.
    /// </summary>
    public class EnemyFSMContext : MonoBehaviour
    {
        [field: SerializeField]
        public Transform Target { get; set; }
        [field: SerializeField]
        public bool FacingRight { get; set; }

        // TODO: Scene에 종속된 플레이어 참조를 얻기 위한 임시 방편. 나중에 반드시 수정할 것.
        void Awake()
        {
            if (Target == null)
                Target = GameObject.FindWithTag("Player").transform;
        }
    }
}
