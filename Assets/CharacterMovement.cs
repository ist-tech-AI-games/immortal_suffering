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
    [SerializeField] private bool onGroundOrPlatform = false; // Flag for ground or platform detection
    [SerializeField] private float moveTimeLeft = 0.0f; // Timer for movement state
    [SerializeField] private bool moveLeft = false; // Flag for moving left
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float jumpForce = 10.0f;
    [SerializeField] private float doubleJumpForce = 5.0f;
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
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the object is a platform or ground
        if (
            (currentState == PlayerState.Jumping || currentState == PlayerState.DoubleJumping) 
            && characterCollider.IsTouchingLayers(LayerMask.GetMask("Ground", "Platform"))
            && rb.linearVelocity.y <= 0.0f // Only consider ground when falling down
            )
        {
            onFeetCollider = collision.collider; // Store the collider of the object we are Standing on
            currentState = PlayerState.Idle; // Reset state to Idle when touching ground
        }
    }
    private void FixedUpdate()
    {
        if (moveTimeLeft > 0.0f && 
            (currentState == PlayerState.Moving || currentState == PlayerState.OnAirMoving))
        {
            rb.linearVelocityX =
                moveSpeed * 
                (currentState == PlayerState.OnAirMoving ? 0.5f : 1.0f) *
                (moveLeft ? -1.0f : 1.0f) *
                 Mathf.Sin(moveTimeLeft); // Move left or right based on the flag

            moveTimeLeft -= Time.fixedDeltaTime; // Decrease the timer
            if (moveTimeLeft <= 0.0f)
            {
                currentState = PlayerState.Idle; // Reset state to Idle when timer ends
            }
        }
    }
    public void Attack()
    {
        // Implement attack logic here
        Debug.Log("Attack performed");
    }

    public void Move(bool[] moveInput)
    {
        // Example movement logic based on input
        if (moveInput[0]) // W: Jump
        {
            if (currentState == PlayerState.Idle || currentState == PlayerState.Moving)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce); // Jump force
                currentState = PlayerState.Jumping;
            }
            else if (currentState == PlayerState.Jumping)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocityX, doubleJumpForce); // Reduce jump force
                currentState = PlayerState.DoubleJumping;
            }
        }
        if (moveInput[1]) // A: Move Left
        {
            moveTimeLeft = 1.0f; // Reset move timer
            moveLeft = true; // Set flag to move left
            if (currentState == PlayerState.Jumping || currentState == PlayerState.DoubleJumping)
            {
                currentState = PlayerState.OnAirMoving; // Set state to OnAirMoving if jumping
            }
            else if (currentState == PlayerState.Idle)
            {
                currentState = PlayerState.Moving; // Set state to Moving if idle
            }
        }
        if (moveInput[3]) // D: Move Right
        {
            moveTimeLeft = 1.0f; // Reset move timer
            moveLeft = false; // Set flag to move right
            if (currentState == PlayerState.Jumping || currentState == PlayerState.DoubleJumping)
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
                !onFeetCollider.IsTouchingLayers(LayerMask.GetMask("Character")))
            {
                Debug.Log("Down Jump: Not touching platform layer");
                return; // Check if touching platform layer
            }
            // Check if player is in a state that allows down jump
            if (currentState == PlayerState.Idle || currentState == PlayerState.Moving)
            {
                // Get Platform Layer's Collider2D
                onFeetCollider.excludeLayers = LayerMask.GetMask("Character"); // Set the collider to trigger to avoid physics issues
                currentState = PlayerState.Jumping; // Set state to Jumping
            }
        }
    }
}
