using UnityEngine;

public class CharacterAttackTrigger : AttackTrigger
{
    public void ExecuteAnimation()
    {
        this.GetComponent<SpriteRenderer>().enabled = true;
    }

    public void EndAnimation()
    {
        this.GetComponent<SpriteRenderer>().enabled = false;
    }

    public override void PerformAttack()
    {
        ExecuteAnimation();
        base.PerformAttack();
    }

    private void RemoveEnemyAHitTrigger(EnemyHitTrigger enemyHitTrigger)
    {
        if (inRangeTriggers.Contains(enemyHitTrigger))
        {
            inRangeTriggers.Remove(enemyHitTrigger);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        collision.GetComponent<EnemyHitTrigger>()?.RegisterOnDestroyed(RemoveEnemyAHitTrigger);
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
        collision.GetComponent<EnemyHitTrigger>()?.UnregisterOnDestroyed(RemoveEnemyAHitTrigger);
    }
}