using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField] private Collider2D platformCollider; // Collider for the platform
    [SerializeField] private float remainingTime;
    [SerializeField] private bool isCharacterDownJumping; // Flag to check if the character is falling
    public bool IsCharacterFalling => remainingTime > 0.0f; // Property to check if the character is falling
    private void Start()
    {
        platformCollider = GetComponent<Collider2D>();
        remainingTime = 0.0f;
    }
    private void FixedUpdate()
    {
        if (remainingTime > 0.0f)
        {
            remainingTime -= Time.fixedDeltaTime;
        }
        else if (isCharacterDownJumping && remainingTime < 0.0f)
        {
            platformCollider.excludeLayers = 0;
            isCharacterDownJumping = false; // Reset the flag when the time runs out
        }
    }

    public void SetExcludeLayers()
    {
        platformCollider.excludeLayers = LayerMask.GetMask("CharacterTerrainInteraction"); // Set the excludeLayers to the given layers
        remainingTime = 0.2f; // Reset the remaining time to 0.2 seconds
        isCharacterDownJumping = true; // Set the flag to true
    }
}
