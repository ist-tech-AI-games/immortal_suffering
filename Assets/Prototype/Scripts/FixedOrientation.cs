using UnityEngine;

namespace ImmortalSuffering.Prototype
{
    public class FixedOrientation : MonoBehaviour
    {
        [SerializeField] private Vector3 euler = Vector3.zero;
        private Quaternion rot;

        void Start()
        {
            rot = Quaternion.Euler(euler);
            
        }

        void FixedUpdate()
        {
            transform.rotation = rot;
        }
    }
}