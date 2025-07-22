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
        private HashSet<AHitTrigger> hitTargets = new();

        void OnEnable()
        {
            hitTargets = new();
        }

        void OnTriggerStay2D(Collider2D collision)
        {
            if (!enabled
                || !collision.TryGetComponent(out AHitTrigger hitTrigger)
                || hitTargets.Contains(hitTrigger)) return;

            Debug.Log($"TriggerStay0: {hitTrigger.name}");
            if( !RaycastTarget(collision))
                return;
            Debug.Log($"TriggerStay1: {hitTrigger.name}");
            
            hitTargets.Add(hitTrigger);
            hitTrigger.OnHit(damage, knockbackRatio, transform);
        }

        // Linecast로 중심 사이에 blocking mask가 없는지만 확인.
        private bool RaycastTarget(Collider2D target)
        {
            if (target == null) return false;

            return !Physics2D.Linecast(transform.position, target.transform.position, blockingMask);
        }
    }
}