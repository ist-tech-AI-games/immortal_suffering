using UnityEngine;

namespace ImmortalSuffering
{
    public class HorizontalMovement : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private Animator animator;
        [SerializeField] private string moveParam = "Movement";
        [SerializeField] private Rigidbody2D rb2d;
        [SerializeField] private float moveSpeed = 5f;

        private int moveParamId;

        void Start()
        {
            moveParamId = Animator.StringToHash(moveParam);
        }

        void FixedUpdate()
        {
            rb2d.MovePosition(rb2d.position + Vector2.right * (moveSpeed * animator.GetInteger(moveParamId) * Time.fixedDeltaTime));
        }
    }
}