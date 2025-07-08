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
}