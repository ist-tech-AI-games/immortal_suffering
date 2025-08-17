using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class TriggerDetector : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private UnityEvent onTargetDetected;
    [SerializeField] private UnityEvent onTargetLeaved;
    [SerializeField] private UnityEvent onAllTargetLeaved;
    private HashSet<Collider2D> overlappingTriggers = new();

    private bool MatchLayer(Collider2D collider2D) =>
        (targetLayer & (1 << collider2D.gameObject.layer)) != 0;

    private void EnterTrigger(Collider2D collider2D)
    {
        onTargetDetected?.Invoke();
        overlappingTriggers.Add(collider2D);
    }

    private void LeaveTrigger(Collider2D collider2D)
    {
        onTargetLeaved?.Invoke();
        overlappingTriggers.Remove(collider2D);
        if (!overlappingTriggers.Any())
            onAllTargetLeaved?.Invoke();
    }

    void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.enabled && MatchLayer(collider2D))
            EnterTrigger(collider2D);
    }

    void OnTriggerStay2D(Collider2D collider2D)
    {
        if (!MatchLayer(collider2D)) return;

        if (collider2D.enabled && !overlappingTriggers.Contains(collider2D))
            EnterTrigger(collider2D);
        if (!collider2D.enabled && overlappingTriggers.Contains(collider2D))
            LeaveTrigger(collider2D);
    }

    void OnTriggerExit2D(Collider2D collider2D)
    {
        if (collider2D.enabled && MatchLayer(collider2D))
            LeaveTrigger(collider2D);
    }
}