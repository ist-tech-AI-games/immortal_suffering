using UnityEngine;

public class CharacterAttackTrigger : AttackTrigger
{
    [SerializeField] private bool isAttacking;
    [SerializeField] private Collider2D attackCollider;  // 공격 중이 아니면 Collider를 비활성화 -> 의도치 않은 상호작용 방지
    [SerializeField] private GameObject enabledObjectOnAttack; // renderer를 외부로 뺄 수 있음 -> Minimap Layer 분리
    private void ExecuteAnimation()
    {
        // this.GetComponent<SpriteRenderer>().enabled = true;
        attackCollider.enabled = true;
        enabledObjectOnAttack.SetActive(true);
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
        // this.GetComponent<SpriteRenderer>().enabled = false;
        attackCollider.enabled = false;
        enabledObjectOnAttack.SetActive(false);
        isAttacking = false;
    }

    public override void PerformAttack()
    {
        ExecuteAnimation();
        base.PerformAttack();
        isAttacking = true;
    }

    protected override void EnterTrigger(AHitTrigger target)
    {
        base.EnterTrigger(target);
        if (target is EnemyHitTrigger enemyHit)
            enemyHit.RegisterOnDestroyed(RemoveEnemyAHitTrigger);
    }

    protected override void ExitTrigger(AHitTrigger target)
    {
        base.ExitTrigger(target);
        if (target is EnemyHitTrigger enemyHit)
            enemyHit.UnregisterOnDestroyed(RemoveEnemyAHitTrigger);
    }

    protected override bool ShouldAttack(AHitTrigger target) =>
        isAttacking && target is EnemyHitTrigger;

    // {
    //     base.OnTriggerEnter2D(collision);
    //     collision.GetComponent<EnemyHitTrigger>()?.RegisterOnDestroyed(RemoveEnemyAHitTrigger);
    //     if (isAttacking)
    //     {
    //         EnemyHitTrigger enemyHitTrigger = collision.GetComponent<EnemyHitTrigger>();
    //         enemyHitTrigger.OnHit(damage, knockbackRatio, transform);
    //     }
    // }

    // public void OnTriggerExit2D(Collider2D collision)
    // {
    //     base.OnTriggerExit2D(collision);
    //     collision.GetComponent<EnemyHitTrigger>()?.UnregisterOnDestroyed(RemoveEnemyAHitTrigger);
    // }
}