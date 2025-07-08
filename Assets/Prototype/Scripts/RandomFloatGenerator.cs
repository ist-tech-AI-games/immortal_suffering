using UnityEngine;
using UnityEngine.Events;

namespace ImmortalSuffering
{
    public class RandomFloatGenerator : MonoBehaviour
    {
        [SerializeField] private Vector2 range;
        [SerializeField] private float period = 2f;
        [SerializeField] private UnityEvent<float> onNumberGenerated;
        float lastInvoked = 0f;

        void FixedUpdate()
        {
            if (Time.fixedTime - lastInvoked > period)
            {
                lastInvoked = Time.fixedTime;
                float min, max;
                if (range.x < range.y)
                {
                    min = range.x; max = range.y;
                }
                else
                {
                    min = range.y; max = range.x;
                }

                onNumberGenerated?.Invoke(Random.Range(min, max));
            }
        }
    }
}