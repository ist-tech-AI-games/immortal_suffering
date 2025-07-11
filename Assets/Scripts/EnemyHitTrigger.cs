using UnityEngine;

public delegate void WhenEnemyDestroyed(EnemyHitTrigger enemyHitTrigger);

public class EnemyHitTrigger : AHitTrigger
{
    [SerializeField] private float remainingHealth = 100.0f; // Health of the enemy

    private event WhenEnemyDestroyed OnDestroyed;

    private void Start()
    {
        OnDestroyed = null; // Initialize the OnDestroyed event
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
        remainingHealth -= damage;
        if (remainingHealth <= 0)
        {
            OnDestroyed?.Invoke(this); // Invoke the callback if registered
            Destroy(gameObject);
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
