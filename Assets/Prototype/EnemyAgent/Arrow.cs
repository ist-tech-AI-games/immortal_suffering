using UnityEngine;

namespace ImmortalSuffering
{
    public class Arrow : MonoBehaviour
    {
        [SerializeField] private float speed;
        [System.Flags]
        enum CollisionType
        {
            Collision = 1,
            Trigger = 2,
        }

        [SerializeField]
        private CollisionType collisionType;

        [SerializeField]
        private LayerMask targetMask;


        private Rigidbody2D rb2d;

        enum ArrowState
        {
            Loaded, Shoot
        }
        private ArrowState arrowState = ArrowState.Loaded;

        void Start()
        {
            rb2d = GetComponent<Rigidbody2D>();
        }

        void FixedUpdate()
        {
            if (arrowState == ArrowState.Shoot)
                rb2d.MovePosition(rb2d.position + (Vector2)transform.right * (speed * Time.fixedDeltaTime));
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (arrowState != ArrowState.Shoot) return;
            if ((collisionType & CollisionType.Collision) != 0
                && (targetMask & (1 << collision.gameObject.layer)) != 0)
                Destroy(gameObject);
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (arrowState != ArrowState.Shoot) return;
            if (enabled
                && (collisionType & CollisionType.Trigger) != 0
                && (targetMask & (1 << collision.gameObject.layer)) != 0)
                Destroy(gameObject);
        }

        public void Shoot()
        {
            arrowState = ArrowState.Shoot;
        }
    }
}
