using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField] private Collider2D platformCollider; // Collider for the platform
    [SerializeField] private float remainingTime;
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
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        CharacterMovement characterMovement = collision.gameObject.GetComponent<CharacterMovement>();
        if (characterMovement) {
            characterMovement.CharacterLandedTriggered(platformCollider); // Set the onFeetCollider to this platform
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        // Clear excludeLayers
        if (remainingTime <= 0.0f && collision.gameObject.GetComponent<CharacterMovement>()) platformCollider.excludeLayers = 0;
    }

    public void SetExcludeLayers(int layers)
    {
        platformCollider.excludeLayers = layers; // Set the excludeLayers to the given layers
        remainingTime = 0.1f; // Reset the remaining time to 0.1 seconds
    }
}
