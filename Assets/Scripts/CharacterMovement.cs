using System;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D rb; // Assuming you have a Rigidbody for physics-based movement
    [SerializeField] private Collider2D characterCollider; // Assuming you have a Collider for collision detection
    [SerializeField] private Collider2D onFeetCollider; // Collider for platform detection
    [Header("State Variables")]
    [SerializeField] private PlayerState currentState = PlayerState.Idle;
    [SerializeField] private int jumpCount = 0; // Flag for jump state
    [SerializeField] private float remainingMoveTime = 0.0f; // Timer for movement state
    [SerializeField] private bool faceIsLeft = false; // Flag for moving left
    [SerializeField] private bool isOnGroundOrPlatform = false; // Flag for ground state
    [SerializeField] private bool isOnEnemy = false; // Flag for ground state
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float moveDelay = 0.3f;
    [SerializeField] private float jumpVelocity = 7.0f;
    [SerializeField] private float doubleJumpVelocity = 5.0f;
    private enum PlayerState
    {
        Idle,
        Moving,
        OnAirMoving,
        Jumping,
        DoubleJumping,
        Attacking,
        AttackedAndStunned,
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        characterCollider = GetComponent<Collider2D>();
        groundMask = LayerMask.GetMask(new string[] { "Ground", "Platform" });
        enemyMask = LayerMask.GetMask(new string[] { "Enemy" });
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        CharacterLandedTriggered(collision.collider); // Trigger landing event
    }
    private LayerMask groundMask;
    private LayerMask enemyMask;
    private RaycastHit2D downwardHit = new RaycastHit2D();
    private void FixedUpdate()
    {
        transform.rotation = Quaternion.identity; // Reset rotation to prevent rotation issues
        Debug.DrawRay(transform.position, Vector2.down * 1.05f, Color.red); // Debug line for ground check
        if (downwardHit = Physics2D.Raycast(transform.position + Vector3.down, Vector2.down, 0.05f, groundMask))
        {
            isOnGroundOrPlatform = true;
        }
        else
        {
            isOnGroundOrPlatform = false;
        }
        if (downwardHit = Physics2D.Raycast(transform.position + Vector3.down, Vector2.down, 0.05f, enemyMask))
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
    }

    // Ground 혹은 Platform에 착지했을 때 호출되는 메소드
    public void CharacterLandedTriggered(Collider2D landedCollider)
    {
        Debug.Log("rg y Velocity: " + rb.linearVelocity.y);
        if (
                (currentState == PlayerState.Jumping ||
                currentState == PlayerState.DoubleJumping ||
                currentState == PlayerState.OnAirMoving)
            && isOnGroundOrPlatform
            )
        {
            onFeetCollider = landedCollider;
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
    }
    public void Attack()
    {
        // Implement attack logic here
        
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
            else if (currentState == PlayerState.Idle)
            {
                currentState = PlayerState.Moving; // Set state to Moving if idle
            }
        }
        if (moveInput[2]) // S: Down Jump
        {
            // Check if touching platform layer
            if (!characterCollider.IsTouchingLayers(LayerMask.GetMask("Platform")) ||
                !onFeetCollider ||
                !onFeetCollider.IsTouchingLayers(LayerMask.GetMask("Character")) ||
                jumpCount >= 1)
            {
                return; // Check if touching platform layer
            }
            // Check if player is in a state that allows down jump
            if (currentState == PlayerState.Idle || currentState == PlayerState.Moving)
            {
                jumpCount = 1; // Set jumping flag
                // Get Platform Layer's Collider2D
                onFeetCollider.excludeLayers = LayerMask.GetMask("Character"); // Set the collider to trigger to avoid physics issues
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
