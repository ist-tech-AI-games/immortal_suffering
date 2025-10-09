using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AttackTrigger : MonoBehaviour
{
    [SerializeField] protected float damage = 10.0f; // Damage dealt by the attack
    [SerializeField] protected float knockbackRatio = 1.0f; // Ratio of knockback applied to the target
    [SerializeField] protected bool isTriggeredOnCollisionWithPlayer = true;
    [SerializeField] protected LayerMask targetLayer; // Layer mask to specify which layers this trigger interacts with
    [SerializeField] protected UnityEvent onAttackPerformed;
    protected HashSet<AHitTrigger> inRangeTriggers = new();

    public virtual void PerformAttack()
    {
        // foreach (var trigger in inRangeTriggers)
        // {
        //     Debug.Log($"Performing attack on {trigger.name} with damage: {damage}, knockback ratio: {knockbackRatio}");
        //     trigger.OnHit(damage, knockbackRatio, transform);
        // }
        // if (inRangeTriggers.Any())
        //     onAttackPerformed?.Invoke();
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out AHitTrigger hitTrigger)) return;

        if (ShouldAttack(hitTrigger) && !inRangeTriggers.Contains(hitTrigger))
            EnterTrigger(hitTrigger);
        if (!ShouldAttack(hitTrigger) && inRangeTriggers.Contains(hitTrigger))
            ExitTrigger(hitTrigger);

        // if (isTriggeredOnCollisionWithPlayer && targetLayer == (targetLayer | (1 << collision.gameObject.layer)))
        // {
        //     PerformAttack();
        // }
    }

    // May Triggered when Object Destroyed at External
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out AHitTrigger hitTrigger) && inRangeTriggers.Contains(hitTrigger))
            ExitTrigger(hitTrigger);
    }

    protected virtual bool ShouldAttack(AHitTrigger target) =>
        enabled
        && target.enabled
        && ((targetLayer & (1 << target.gameObject.layer)) != 0);

    protected virtual void EnterTrigger(AHitTrigger target)
    {
        inRangeTriggers.Add(target);
        target.OnHit(damage, knockbackRatio, transform);
        onAttackPerformed?.Invoke();
    }

    protected virtual void ExitTrigger(AHitTrigger target)
    {
        inRangeTriggers.Remove(target);
    }
}
