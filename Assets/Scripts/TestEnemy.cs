using UnityEngine;

public class Test : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        CharacterMovement characterMovement = collision.gameObject.GetComponent<CharacterMovement>();
        if (characterMovement) {
            characterMovement.CharacterAttackedTriggered(3.0f, transform); // Simulate an attack with 3 damage
        }
    }
}
