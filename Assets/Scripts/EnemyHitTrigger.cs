using UnityEngine;
using UnityEngine.Events;

public delegate void WhenEnemyDestroyed(EnemyHitTrigger enemyHitTrigger);

public class EnemyHitTrigger : AHitTrigger
{
    [SerializeField] private float remainingHealth = 100.0f; // Health of the enemy
    [SerializeField] private float knockbackMultiplier = 0f;
    [SerializeField] private UnityEvent<float> onHealthChanged; // Remaining health
    [SerializeField] private UnityEvent onDied;

    private event WhenEnemyDestroyed OnDestroyed;
    private Rigidbody2D rb2d;

    private void Start()
    {
        OnDestroyed = null; // Initialize the OnDestroyed event
        rb2d = GetComponentInParent<Rigidbody2D>();
        onHealthChanged?.Invoke(remainingHealth);
    }

    public void RegisterOnDestroyed(WhenEnemyDestroyed callback)
    {
        OnDestroyed += callback; // Register a callback to be invoked when the object is destroyed
    }
    public void UnregisterOnDestroyed(WhenEnemyDestroyed callback)
    {
        OnDestroyed -= callback; // Unregister the callback
    }

    public override void OnHit(float damage, float knockbackRatio, Transform attacker)
    {
        if (!enabled) return; // 비활성화로 피해 차단 가능.
        remainingHealth -= damage;

        // TODO: Extract knockback logic to somewhere else
        rb2d?.AddForce(
            new Vector2(
                attacker.position.x > transform.position.x ? -1.0f : 1.0f, // Determine knockback direction based on enemy position
                0f // 임시로 수직 넉백 제거
            ) * knockbackMultiplier * knockbackRatio, ForceMode2D.Impulse // Apply knockback force
        );

        onHealthChanged?.Invoke(remainingHealth);
        if (remainingHealth <= 0)
        {
            OnDestroyed?.Invoke(this); // Invoke the callback if registered
            onDied?.Invoke();  // 사망 애니메이션 대응을 위해 임시 변경.
            // Destroy(gameObject);
        }
    }

    // 캐릭터와 적이 부딪혔을 때 호출되는 메소드, 테스트용
    void OnCollisionEnter2D(Collision2D collision)
    {
        CharacterMovement characterMovement = collision.gameObject.GetComponent<CharacterMovement>();
        if (characterMovement) {
            characterMovement.CharacterAttackedTriggered(3.0f, 1.0f, transform);
        }
    }
}
