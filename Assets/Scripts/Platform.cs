using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField] private Collider2D platformCollider; // Collider for the platform
    private void Start()
    {
        platformCollider = GetComponent<Collider2D>();
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
        platformCollider.excludeLayers = 0;
    }
}
