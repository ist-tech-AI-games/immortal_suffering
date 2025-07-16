using UnityEngine;
using UnityEngine.Events;

public class TriggerDetector : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private UnityEvent onTargetDetected;
    [SerializeField] private UnityEvent onTargetLeaved;
    private Collider2D detector;
    private int detectedCnt = 0;
    private ContactFilter2D filter;
    private Collider2D[] results = new Collider2D[8];

    void Start()
    {
        detector = GetComponent<Collider2D>();
        filter = new ContactFilter2D().NoFilter();
        filter.SetLayerMask(targetLayer);
        filter.useTriggers = true;
    }

    private void FixedUpdate()
    {
        int currentDetected = Physics2D.OverlapCollider(detector, filter, results);

        if (detectedCnt == 0 && currentDetected > 0)
        {
            onTargetDetected?.Invoke();
        }

        if (detectedCnt > 0 && currentDetected == 0)
        {
            onTargetLeaved?.Invoke();
        }

        detectedCnt = currentDetected;
    }
}