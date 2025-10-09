using UnityEngine;

public class ChararcterFootAttackTrigger : AttackTrigger
{
    [SerializeField] private float minHeightDiff = .5f;
    protected override bool ShouldAttack(AHitTrigger target) =>
        base.ShouldAttack(target)
        && target.transform.position.y < transform.position.y - minHeightDiff;
}