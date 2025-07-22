using UnityEngine;
using UnityEngine.Events;

namespace ImmortalSuffering
{
    public class Goal : MonoBehaviour
    {
        [SerializeField] private LayerMask targetLayer;
        [SerializeField] private UnityEvent onGoalReached;

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (enabled && ((1 << collision.gameObject.layer) & targetLayer) != 0)
                CompleteGoal();
        }

        private void CompleteGoal()
        {
            Time.timeScale = 0f;
            onGoalReached.Invoke();
        }
    }
}