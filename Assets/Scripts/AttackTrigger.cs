using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AttackTrigger : MonoBehaviour
{
    [SerializeField] protected float damage = 10.0f; // Damage dealt by the attack
    [SerializeField] protected float knockbackRatio = 1.0f; // Ratio of knockback applied to the target
    [SerializeField] protected bool isTriggeredOnCollisionWithPlayer = true;
    [SerializeField] protected LayerMask targetLayer; // Layer mask to specify which layers this trigger interacts with
    [SerializeField] protected List<AHitTrigger> inRangeTriggers;
    [SerializeField] protected UnityEvent onAttackPerformed;
    public virtual void PerformAttack()
    {
        for (int i = inRangeTriggers.Count - 1; i >= 0; i--)
        {
            Debug.Log($"Performing attack on {inRangeTriggers[i].name} with damage: {damage}, knockback ratio: {knockbackRatio}");
            inRangeTriggers[i].OnHit(damage, knockbackRatio, transform);
        }
        if (inRangeTriggers.Count > 0)
            onAttackPerformed?.Invoke();
    }
    private void Start()
    {
        inRangeTriggers = new List<AHitTrigger>();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!enabled) return;
        var hitTrigger = collision.GetComponent<AHitTrigger>();
        if (hitTrigger != null)
        {
            // Add the hit trigger to the in-range triggers list
            inRangeTriggers.Add(hitTrigger);
        }

        if (isTriggeredOnCollisionWithPlayer && targetLayer == (targetLayer | (1 << collision.gameObject.layer)))
        {
            PerformAttack();
        }
    }

    // May Triggered when Object Destroyed at External
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (!enabled) return;
        var outTrigger = collision.GetComponent<AHitTrigger>();
        if (outTrigger != null)
        {            // Remove the hit trigger from the in-range triggers list
            inRangeTriggers.Remove(outTrigger);
        }
    }
}
