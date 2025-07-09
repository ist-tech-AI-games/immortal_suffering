using UnityEngine;

public class CharacterAttackTrigger : AttackTrigger
{
    [SerializeField] private bool isAttacking;
    private void ExecuteAnimation()
    {
        this.GetComponent<SpriteRenderer>().enabled = true;
    }

    private void RemoveEnemyAHitTrigger(EnemyHitTrigger enemyHitTrigger)
    {
        if (inRangeTriggers.Contains(enemyHitTrigger))
        {
            inRangeTriggers.Remove(enemyHitTrigger);
        }
    }

    public void EndAttack()
    {
        this.GetComponent<SpriteRenderer>().enabled = false;
        isAttacking = false;
    }

    public override void PerformAttack()
    {
        ExecuteAnimation();
        base.PerformAttack();
        isAttacking = true;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        collision.GetComponent<EnemyHitTrigger>()?.RegisterOnDestroyed(RemoveEnemyAHitTrigger);
        if (isAttacking)
        {
            EnemyHitTrigger enemyHitTrigger = collision.GetComponent<EnemyHitTrigger>();
            enemyHitTrigger.OnHit(damage, transform);
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
        collision.GetComponent<EnemyHitTrigger>()?.UnregisterOnDestroyed(RemoveEnemyAHitTrigger);
    }
}