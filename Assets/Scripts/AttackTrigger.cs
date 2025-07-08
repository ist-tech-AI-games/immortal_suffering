using System.Collections.Generic;
using UnityEngine;

public class AttackTrigger : MonoBehaviour
{
    [SerializeField] protected float damage = 10.0f; // Damage dealt by the attack
    [SerializeField] protected bool isTriggeredOnCollisionWithPlayer = true;
    public virtual void PerformAttack()
    {
        inRangeTriggers.ForEach(trigger => trigger.OnHit(damage, transform));
    }
    private List<AHitTrigger> inRangeTriggers;
    private void Start()
    {
        inRangeTriggers = new List<AHitTrigger>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        var hitTrigger = collision.GetComponent<AHitTrigger>();
        if (hitTrigger != null)
        {
            // Add the hit trigger to the in-range triggers list
            inRangeTriggers.Add(hitTrigger);
        }

        if (isTriggeredOnCollisionWithPlayer && collision.IsTouchingLayers(LayerMask.GetMask("Player")))
        {
            PerformAttack();
        }
    }

    // May Triggered when Object Destroyed at External
    public void OnTriggerExit2D(Collider2D collision)
    {
        var outTrigger = collision.GetComponent<AHitTrigger>();
        if (outTrigger != null)
        {            // Remove the hit trigger from the in-range triggers list
            inRangeTriggers.Remove(outTrigger);
        }
    }
}
