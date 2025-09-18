using UnityEngine;

namespace ImmortalSuffering.Prototype
{
    public class VelocityModifier : MonoBehaviour
    {
        // (a.x * x.x + b.x, a.y * x.y + b.y) 방식으로 속도 변화.
        // 아래 기본 설정을 사용하면 위로 튀어오르는 효과가 구현됨.
        [SerializeField] private Vector2 coefficient = Vector2.right;
        [SerializeField] private Vector2 constant = Vector2.up;
        private Rigidbody2D rb2d;

        void Awake()
        {
            rb2d = GetComponentInParent<Rigidbody2D>();
        }

        public void Apply() => Apply(coefficient, constant);

        public void Apply(Vector2 coefficient, Vector2 constant)
        {
            Vector2 vel = rb2d.linearVelocity;
            vel.x = coefficient.x * vel.x + constant.x;
            vel.y = coefficient.y * vel.y + constant.y;
            rb2d.linearVelocity = vel;
        }
    }
}