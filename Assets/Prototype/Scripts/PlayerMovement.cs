using UnityEngine;
using UnityEngine.InputSystem;

namespace ImmortalSuffering.Prototype
{
    /// <summary>
    /// 플레이어 이동의 최소 구현.
    /// 바깥의 PlayerInput과는 달리, 적 생성/Agent 기능 테스트만을 위해 사용하는 간단한 구현.
    /// 실제 이동 로직에는 사용하지 않음.
    /// </summary>
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rb2d;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float jumpForce = 100f;
        [SerializeField] private float jumpThreshold = .2f;
        private Vector2 inputVec = Vector2.zero;

        enum JumpState { None, Requested, Performed };
        private JumpState jumpState = JumpState.None;

        public void OnMove(InputValue inputvalue)
        {
            inputVec = inputvalue.Get<Vector2>();
            if (jumpState == JumpState.None && inputVec.y > jumpThreshold) jumpState = JumpState.Requested;
            else if (jumpState == JumpState.Performed && inputVec.y < jumpThreshold) jumpState = JumpState.None;
        }

        private void FixedUpdate()
        {
            rb2d.linearVelocityX = inputVec.x * moveSpeed;
            if (jumpState == JumpState.Requested)
            {
                rb2d.AddForce(Vector2.up * jumpForce);
                jumpState = JumpState.Performed;
            }
        }
    }
}