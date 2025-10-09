using UnityEngine;
using UnityEngine.Events;

namespace ImmortalSuffering
{
    public class AimTarget : MonoBehaviour
    {
        [SerializeField] private Vector2 targetOffset = Vector2.zero;
        [SerializeField] private UnityEvent<float> onAngleSet;
        private Transform target;

        void FixedUpdate()
        {
            // 성능 최적화가 필요하다고 판단되면 일정 비율의 프레임을 생략할 것.
            UpdateAngle();
        }

        public void UpdateAngle()
        {
            if (target == null) return;

            onAngleSet?.Invoke(Vector2.SignedAngle(Vector2.right, target.position + (Vector3)targetOffset - transform.position));
        }

        public void SetTarget(Transform target)
        {
            this.target = target;
        }

        public void UnsetTarget()
        {
            target = null;
        }
    }
}