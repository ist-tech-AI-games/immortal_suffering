using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ImmortalSuffering
{
    public class ContinuousDamage : MonoBehaviour
    {
        [SerializeField] private LayerMask targetMask;
        [System.Serializable]
        struct DamageInfo
        {
            public float Damage;
            public float KnockbackRatio;
        }
        [SerializeField] private DamageInfo dmgPerHit = new DamageInfo { Damage = 5, KnockbackRatio = 0 };
        [SerializeField] private DamageInfo firstDmg = new DamageInfo { Damage = 5, KnockbackRatio = 1 };
        [SerializeField] private int fixedFramePerHit = 2;
        [SerializeField] private float damagePeriod = .8f;
        [SerializeField] private bool hitOnEnter = false;

        private int tick = 0;
        private Dictionary<AHitTrigger, float> caughtTriggers = new();

        void FixedUpdate()
        {
            if (tick++ % fixedFramePerHit != 0) return;

            foreach (var trig in caughtTriggers.Keys.ToArray())
            {
                if (Time.fixedTime - caughtTriggers[trig] > damagePeriod)
                {
                    trig.OnHit(dmgPerHit.Damage, dmgPerHit.KnockbackRatio, transform);
                    caughtTriggers[trig] = Time.fixedTime;
                }
            }
        }

        private bool IsInLayerMask(GameObject go) => ((1 << go.layer) & targetMask) != 0;

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (IsInLayerMask(collision.gameObject) && collision.TryGetComponent(out AHitTrigger hitTrigger))
            {
                if (hitOnEnter)
                    hitTrigger.OnHit(firstDmg.Damage, firstDmg.KnockbackRatio, transform);
                caughtTriggers[hitTrigger] = Time.fixedTime;
            }
        }

        void OnTriggerExit2D(Collider2D collision)
        {
            if (IsInLayerMask(collision.gameObject) && collision.TryGetComponent(out AHitTrigger hitTrigger))
            {
                caughtTriggers.Remove(hitTrigger);
            }
        }
    }
}