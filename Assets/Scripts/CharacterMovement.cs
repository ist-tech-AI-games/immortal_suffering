using ImmortalSuffering;
using Unity.Mathematics;
using Unity.Mathematics.Geometry;
using UnityEngine;
using UnityEngine.Events;

public enum PlayerState
{
    Idle,
    Moving,
    OnAirMoving,
    Jumping,
    DoubleJumping,
    AttackedAndStunned,
}

public enum MoveStatus
{
    IDLE = 0,
    Moving = 1,
    ONAIR = 2,
    STUNNED = 3
}
public class CharacterMovement : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D rb; // Assuming you have a Rigidbody for physics-based movement
    [SerializeField] private Collider2D characterCollider; // Assuming you have a Collider for collision detection
    [SerializeField] private Platform onFeetPlatform; // Collider for platform detection
    [SerializeField] private CharacterAttackSystem characterAttackSystem;
    [Header("State Variables")]
    [SerializeField] private PlayerState _currentState; // Current state of the character
    public PlayerState currentState
    {
        get
        {
            return _currentState;
        }
        private set
        {
            _currentState = value;
            charSpriteRenderer.flipX = !faceIsLeft;
            switch (value)
            {
                case PlayerState.Idle:
                    animatorParameterSetter.SetInteger((int)MoveStatus.IDLE);
                    return;
                case PlayerState.Moving:
                    animatorParameterSetter.SetInteger((int)MoveStatus.Moving);
                    return;
                case PlayerState.OnAirMoving:
                    animatorParameterSetter.SetInteger((int)MoveStatus.ONAIR);
                    return;
                case PlayerState.Jumping:
                    animatorParameterSetter.SetInteger((int)MoveStatus.ONAIR);
                    return;
                case PlayerState.DoubleJumping:
                    animatorParameterSetter.SetInteger((int)MoveStatus.ONAIR);
                    return;
                case PlayerState.AttackedAndStunned:
                    animatorParameterSetter.SetInteger((int)MoveStatus.STUNNED);
                    charSpriteRenderer.flipX = faceIsLeft;
                    return;
            }
        }
    }

    [SerializeField] private SpriteRenderer charSpriteRenderer;
    [SerializeField] private AnimatorParameterSetter animatorParameterSetter; // Animator parameter setter for state changes
    [SerializeField] private int jumpCount; // Flag for jump state
    [SerializeField] private float remainingMoveTime; // Timer for movement state
    [SerializeField] private bool faceIsLeft; // Flag for moving left
    [SerializeField] private bool isGrabbed;
    [SerializeField] private bool isOnGroundOrPlatform; // Flag for ground state
    [SerializeField] private bool isOnEnemy; // Flag for ground state
    private Vector2 beforeSpeed; // Used for Wall Bounce
    [field: SerializeField] public float damageGot { get; private set; }
    [SerializeField] private float attackAnimationRemainingTime; // Timer for attack animation
    [SerializeField] private bool attackingFlag;
    [Header("Movement Settings - Set these in the Inspector")]
    [SerializeField] private float moveDelay;
    [field: SerializeField] public float moveSpeed { get; set; }
    [field: SerializeField] public float jumpVelocity { get; set; }
    [field: SerializeField] public float doubleJumpVelocity { get; set; }
    [Header("Events")]
    [SerializeField] private UnityEvent<float> onDamageChanged;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        characterCollider = GetComponent<Collider2D>();
        characterAttackSystem = GetComponent<CharacterAttackSystem>();
        groundMask = LayerMask.GetMask(new string[] { "Wall", "Ground", "Platform" });
        enemyMask = LayerMask.GetMask(new string[] { "EnemyAtkHit" });

        //Variable Initialization
        rb.linearVelocity = Vector2.zero; // Reset velocity
        currentState = PlayerState.Idle; // Start in Idle state
        jumpCount = 0; // Reset jump count
        remainingMoveTime = 0.0f; // Reset move timer
        faceIsLeft = false; // Start facing right
        isOnGroundOrPlatform = false; // Reset ground state
        isOnEnemy = false; // Reset enemy state
        onFeetPlatform = null; // Reset onFeetPlatform
        damageGot = 0.0f; // Reset damage taken
        onDamageChanged?.Invoke(damageGot);

        Physics2D.queriesHitTriggers = false; // Disable trigger queries for raycasts
    }

    private LayerMask groundMask;
    private LayerMask enemyMask;
    private RaycastHit2D downwardHit = new RaycastHit2D();
    private RaycastHit2D enemyFeetHit = new RaycastHit2D();
    private void FixedUpdate()
    {
        transform.rotation = Quaternion.identity; // Reset rotation to prevent rotation issues

        // Debug.DrawRay(transform.position + Vector3.down * 1.25f + Vector3.right * 0.48f, Vector2.left * 0.96f, Color.red); // Draw ray for debugging
        // Raycast 처리 - 현재 캐릭터의 발 아래 무언가 있는지 확인
        if (downwardHit = Physics2D.Raycast(transform.position + Vector3.down * 1.25f + Vector3.right * 0.48f, Vector2.left, 0.96f, groundMask))
        {
            isOnGroundOrPlatform = true;
        }
        else
        {
            isOnGroundOrPlatform = false;
        }
        if (enemyFeetHit = Physics2D.Raycast(transform.position + Vector3.down * 1.25f + Vector3.right * 0.48f, Vector2.left, 0.96f, enemyMask))
        {
            isOnEnemy = true;
        }
        else
        {
            isOnEnemy = false;
        }
        // Move 처리
        if (remainingMoveTime > 0.0f &&
            (currentState == PlayerState.Moving || currentState == PlayerState.OnAirMoving))
        {
            rb.linearVelocityX =
                moveSpeed *
                (currentState == PlayerState.OnAirMoving ? 0.5f : 1.0f) *
                (faceIsLeft ? -1.0f : 1.0f) *
                 Mathf.Sin(remainingMoveTime / moveDelay); // Move left or right based on the flag

            remainingMoveTime -= Time.fixedDeltaTime; // Decrease the timer
            if (remainingMoveTime <= 0.0f)
            {
                if (jumpCount >= 1 && (currentState == PlayerState.Jumping ||
                                        currentState == PlayerState.DoubleJumping ||
                                        currentState == PlayerState.OnAirMoving))
                {
                    currentState = PlayerState.Jumping;
                }
                else if (currentState == PlayerState.Idle || currentState == PlayerState.Moving)
                {
                    currentState = PlayerState.Idle;
                }
            }
        }

        // 캐릭터 점프 후 착지 처리
        if ((currentState == PlayerState.Jumping ||
                currentState == PlayerState.DoubleJumping ||
                currentState == PlayerState.OnAirMoving)
            && isOnGroundOrPlatform && rb.linearVelocity.y <= 0.0f
            )
        {
            onFeetPlatform = downwardHit.transform.gameObject.GetComponent<Platform>(); // Check if the raycast hit a platform
            Debug.Log("Character landed on: " + downwardHit.transform.gameObject.name);

            jumpCount = 0; // Reset jump count
            if (remainingMoveTime > 0.0f)
            {
                currentState = PlayerState.Moving; // Set state to Moving if still moving
            }
            else
            {
                currentState = PlayerState.Idle; // Reset state to Idle when not moving
            }
        }

        // 캐릭터가 있을 때 발 아래 뭐 있는지 처리
        if (currentState == PlayerState.Idle || currentState == PlayerState.Moving)
        {
            // onFeetPlatform = downwardHit.transform.gameObject.GetComponent<Platform>(); 
            // Debug.Log("Chracter is on: " + downwardHit.transform.gameObject.name);
        }

        // 피격 처리
        if (currentState == PlayerState.AttackedAndStunned && !isGrabbed)
        {
            beforeSpeed = rb.linearVelocity; // Store the velocity before applying knockback
            rb.linearVelocity *= 0.95f; // Apply a slight damping effect to the velocity
            if (rb.linearVelocity.magnitude < 0.5f)
            {
                rb.linearVelocity = Vector2.zero; // Reset velocity
                currentState = PlayerState.Idle; // Reset state to Idle if velocity is low
            }
        }

        // Attack 애니메이션 처리
        if (attackAnimationRemainingTime > 0.0f)
        {
            attackAnimationRemainingTime -= Time.fixedDeltaTime; // Decrease the attack animation timer
            if (attackAnimationRemainingTime <= 0.0f)
            {
                attackingFlag = false; // Reset the attacking flag
                characterAttackSystem.EndAttack(); // End the attack animation
            }
        }
    }

    // Character이 피격되었을 때 호출되는 메소드
    public void CharacterAttackedTriggered(float damage, float knockbackRatio, Transform enemyPosition)
    {
        damageGot += damage; // Accumulate damage
        onDamageChanged?.Invoke(damageGot);
        rb.AddForce(
            new Vector2(
                transform.position.x - enemyPosition.position.x, // Determine knockback direction based on enemy position
                transform.position.y - enemyPosition.position.y
            ).normalized * knockbackRatio * math.sqrt(damageGot) * 10 * math.sqrt(2), ForceMode2D.Impulse // Apply knockback force, 200% 일 때 200의 힘이 가해지는 것으로 수치 설정
        );
        if (knockbackRatio > .02f)
            currentState = PlayerState.AttackedAndStunned; // Set state to AttackedAndStunned
    }

    public void CharacterGrabbedTriggered(bool isGrabbed)
    {
        if (isGrabbed)
        {
            currentState = PlayerState.AttackedAndStunned;
            this.isGrabbed = true;
        }
        else
        {
            this.isGrabbed = false;
            currentState = PlayerState.Idle;
        }
    }

    [SerializeField] private GameObject wallHitParticle;
    // TestBounceWall에서 호출되는 메소드
    public void CharacterCollisionTriggered(Collision2D collision, float bounceRate, bool isPlatform = false)
    {
        if (currentState != PlayerState.AttackedAndStunned) return;
        if (isPlatform && beforeSpeed.y > 0.0f) return;

        Instantiate(wallHitParticle).transform.position = collision.transform.position;
        var velocity = beforeSpeed.magnitude;
        var direction = Vector2.Reflect(beforeSpeed.normalized, collision.contacts[0].normal);
        rb.linearVelocity = direction * velocity * bounceRate; // Apply the reflected velocity
    }

    public void Attack(AttackDirection direction)
    {
        if (attackingFlag || currentState == PlayerState.AttackedAndStunned) return; // Prevent multiple attacks at the same time

        attackAnimationRemainingTime = characterAttackSystem.PerformAttack(direction); // Perform attack based on direction
        attackingFlag = true;
    }

    // Character 움직임 처리
    public void Move(bool[] moveInput)
    {
        // Example movement logic based on input
        if (moveInput[0]) // W: Jump
        {
            if (currentState == PlayerState.Idle)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpVelocity); // Jump force
                currentState = PlayerState.Jumping;
                jumpCount = 1;
            }
            else if (currentState == PlayerState.Moving)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpVelocity); // Jump force
                currentState = PlayerState.OnAirMoving;
                jumpCount = 1;
            }
            else if (currentState == PlayerState.OnAirMoving && jumpCount == 1)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocityX, doubleJumpVelocity); // Jump force
                currentState = PlayerState.OnAirMoving;
                jumpCount = 2; // Reset jump count for double jump
            }
            else if (currentState == PlayerState.Jumping && jumpCount == 1)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocityX, doubleJumpVelocity); // Reduce jump force
                currentState = PlayerState.DoubleJumping;
                jumpCount = 2; // Reset jump count for double jump
            }
        }
        if (moveInput[1] || moveInput[3]) // A: Move Left, D: Move Right
        {
            remainingMoveTime = moveDelay; // Reset move timer
            faceIsLeft = moveInput[1] == true; // Set flag to move left
            if (currentState == PlayerState.Jumping ||
                currentState == PlayerState.DoubleJumping ||
                currentState == PlayerState.OnAirMoving)
            {
                currentState = PlayerState.OnAirMoving; // Set state to OnAirMoving if jumping
            }
            else if (currentState == PlayerState.Idle || currentState == PlayerState.Moving)
            {
                currentState = PlayerState.Moving; // Set state to Moving if idle
            }
        }
        if (moveInput[2]) // S: Down Jump
        {
            // Check if player is in a state that allows down jump
            if (currentState == PlayerState.Idle || currentState == PlayerState.Moving)
            {
                jumpCount = 1; // Set jumping flag
                if (downwardHit.transform.TryGetComponent(out onFeetPlatform))
                    onFeetPlatform.SetExcludeLayers();
                if (currentState == PlayerState.Idle)
                {
                    currentState = PlayerState.Jumping; // Set state to Jumping
                }
                else if (currentState == PlayerState.Moving)
                {
                    currentState = PlayerState.OnAirMoving; // Set state to OnAirMoving
                }
            }
        }
    }
}
