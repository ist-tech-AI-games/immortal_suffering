using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ImmortalSuffering
{
    // 시야 판정을 위한 컴포넌트. 시야 Collider가 플레이어의 Layer만 감지하도록 설정할 것.
    public class SightManager : MonoBehaviour
    {
        [SerializeField] private LayerMask targetMask;
        [SerializeField] private LayerMask blockingMask;
        [SerializeField] private float raycastAngle = 7f; // approx of arcsin(0.7 / 6), in deg.
        [SerializeField] private Vector2 angleLimit = new Vector2(-15f, 195f);
        [SerializeField] private UnityEvent<Transform> onTargetEnter, onTargetExit;
        private Collider2D sightCollider;
        private ContactFilter2D targetFilter;
        private HashSet<Transform> visibleTargets = new();

        void Start()
        {
            sightCollider = GetComponent<Collider2D>();
            targetFilter = new ContactFilter2D().NoFilter();
            targetFilter.SetLayerMask(targetMask);
        }

        void FixedUpdate()
        {
            HashSet<Transform> currentVisible = new();

            Collider2D[] detected = new Collider2D[4];
            int cnt = Physics2D.OverlapCollider(sightCollider, targetFilter, detected);

            for (int i = 0; i < cnt; i++)
            {
                var targetCol = detected[i];
                bool a = CheckAngleLimit(targetCol), r = RaycastTarget(targetCol);
                // if (CheckAngleLimit(targetCol) && RaycastTarget(targetCol))
                if (a && r)
                {
                    Transform target = targetCol.transform;
                    currentVisible.Add(target);

                    // 새로 들어온 대상
                    if (!visibleTargets.Contains(target))
                    {
                        onTargetEnter?.Invoke(target);
                    }

                    Debug.DrawLine(transform.position, target.position, Color.green);
                }
                else
                    if (a) Debug.DrawLine(transform.position, targetCol.transform.position, Color.blue);
                else if (r) 
                     Debug.DrawLine(transform.position, targetCol.transform.position, Color.yellow);
                     else Debug.DrawLine(transform.position, targetCol.transform.position, Color.red);
            }

            // 나간 대상들
            foreach (var target in visibleTargets)
            {
                if (!currentVisible.Contains(target))
                {
                    onTargetExit?.Invoke(target);
                }
            }

            visibleTargets = currentVisible;
        }

        // 요청 시 Physics2D로 감지 vs OnCollisionStay2D 중 성능 부담이 적은 걸 고민하다 전자를 채택.
        public Transform TryGetTarget()
        {
            foreach (var target in visibleTargets)
                return target;
            return null;
        }

        // 현재는 transform.right를 기준으로 삼으므로, -90 ~ 270도 사이의 범위로 각도를 조정.
        private float NormalizeAngle(float angle) => ((((angle + 90f) % 360f) + 360f) % 360f) - 90f;

        // 대상과의 각도가 범위 내인지 검사.
        private bool CheckAngleLimit(Collider2D target)
        {
            // heuristic: 너무 가까우면 각도를 따지지 않음.
            if ((target.transform.position - transform.position).sqrMagnitude < .5f * .5f) return true;
            float angle = NormalizeAngle(Vector2.SignedAngle(Vector2.right, target.transform.position - transform.position));
            return (angleLimit.x - angle) * (angleLimit.y - angle) < 0f;
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