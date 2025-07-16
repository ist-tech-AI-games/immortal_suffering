using System.Collections.Generic;
using UnityEngine;

namespace ImmortalSuffering
{
    // 기존 AttackTrigger와 달리 벽에 막히는(시야와 비슷한) 로직이 필요하므로 별도로 구현함.
    public class ExplosionDamage : MonoBehaviour
    {
        [SerializeField] private LayerMask targetMask;
        [SerializeField] private LayerMask blockingMask;
        [SerializeField] private float damage = 100f;
        [SerializeField] private float knockbackRatio = 1f;
        [SerializeField] private float raycastAngle = 7f; // approx of arcsin(0.7 / 6), in deg.
        private Collider2D explosionCollider;
        private ContactFilter2D targetFilter;
        private HashSet<AHitTrigger> hitTargets = new();

        void Start()
        {
            explosionCollider = GetComponent<Collider2D>();
            targetFilter = new ContactFilter2D().NoFilter();
            targetFilter.SetLayerMask(targetMask);
            targetFilter.useTriggers = true;
        }

        void OnEnable()
        {
            hitTargets = new();
        }

        int frame = 0;
        void FixedUpdate()
        {
            if (frame++ % 3 != 0) return;

            Collider2D[] detected = new Collider2D[16];
            int cnt = Physics2D.OverlapCollider(explosionCollider, targetFilter, detected);

            for (int i = 0; i < cnt; i++)
            {
                var targetCol = detected[i];
                if (   !targetCol.TryGetComponent(out AHitTrigger hitTrigger)
                    || hitTargets.Contains(hitTrigger)  // 한 번의 폭발에 2회 이상 피격되지 않음.
                    || !RaycastTarget(targetCol))
                    continue;

                hitTargets.Add(hitTrigger);
                hitTrigger.OnHit(damage, knockbackRatio, transform);
            }
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