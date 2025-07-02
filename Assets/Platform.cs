using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField] private Collider2D platformCollider; // Collider for the platform
    private void Start()
    {
        platformCollider = GetComponent<Collider2D>();
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        // Clear excludeLayers
        platformCollider.excludeLayers = 0;
    }
}
