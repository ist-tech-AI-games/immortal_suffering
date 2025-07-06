using UnityEngine;

namespace ImmortalSuffering
{
    // 시야 판정을 위한 컴포넌트. 시야 Collider가 플레이어의 Layer만 감지하도록 설정할 것.
    public class SightManager : MonoBehaviour
    {
        [SerializeField] private LayerMask targetMask;
        [SerializeField] private LayerMask blockingMask;
        [SerializeField] private float raycastAngle = 7f; // approx of arcsin(0.7 / 6), in deg.
        private Collider2D sightCollider;
        private ContactFilter2D targetFilter;

        void Start()
        {
            sightCollider = GetComponent<Collider2D>();
            targetFilter = new ContactFilter2D().NoFilter();
            targetFilter.SetLayerMask(targetMask);
        }

        // test
        void FixedUpdate()
        {
            var t = TryGetTarget();
            if (t != null)
            {
                Debug.DrawLine(transform.position, t.position);
            }
        }

        // 요청 시 Physics2D로 감지 vs OnCollisionStay2D 중 성능 부담이 적은 걸 고민하다 전자를 채택.
        public Transform TryGetTarget()
        {
            Collider2D[] detected = new Collider2D[4];
            int cnt = Physics2D.OverlapCollider(sightCollider, targetFilter, detected);

            for (int i = 0; i < cnt; i++)
            {
                var targetCol = detected[i];
                if (RaycastTarget(targetCol)) return targetCol.transform;
            }
            return null;
        }

        // heuristic: 대상의 중심을 향해 raycast 검사. 초기 기획상 플레이어 키와 시야 반지름이 약 2:6이므로,
        // 중심 raycast 실패 시 arcsin(0.7/6)(조정 가능)만큼 각도를 양방향으로 조정한 raycast를 각각 시도.
        private bool RaycastTarget(Collider2D target)
        {
            if (target == null) return false;
            Vector2 dir = target.transform.position - transform.position;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 100f, targetMask | blockingMask);
            if (hit.collider == target) return true;

            dir = Quaternion.AngleAxis(raycastAngle, Vector3.forward) * dir;

            hit = Physics2D.Raycast(transform.position, dir, 100f, targetMask | blockingMask);
            if (hit.collider == target) return true;

            dir = Quaternion.AngleAxis(-2 * raycastAngle, Vector3.forward) * dir;

            hit = Physics2D.Raycast(transform.position, dir, 100f, targetMask | blockingMask);
            return hit.collider == target;
        }
    }

}