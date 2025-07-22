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

    void OnTriggerEnter2D(Collider2D collision)
    {
        if ((targetLayer & (1 << collision.gameObject.layer)) != 0)
        {
            onTargetDetected?.Invoke();
            overlappingTriggers.Add(collision);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if ((targetLayer & (1 << collision.gameObject.layer)) != 0)
        {
            onTargetLeaved?.Invoke();
            overlappingTriggers.Remove(collision);
            if (!overlappingTriggers.Any())
                onAllTargetLeaved?.Invoke();
        }
    }
}