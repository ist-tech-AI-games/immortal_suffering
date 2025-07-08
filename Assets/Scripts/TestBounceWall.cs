using UnityEngine;

public class TestBounceWall : MonoBehaviour
{
    [SerializeField] private float bounceRate;
    [SerializeField] private bool isPlatform;
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<CharacterMovement>(out CharacterMovement cm))
        {
            cm.CharacterCollisionTriggered(collision, bounceRate, isPlatform); // Trigger character collision event
        }
    }
}
