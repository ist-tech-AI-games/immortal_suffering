using UnityEngine;

public class CharacterAttackSystem : MonoBehaviour
{
    private AttackDirection beforeAttackDirection;
    [SerializeField] private float attackAnimationDuration = 0.5f; // Duration of the attack animation
    [SerializeField] private CharacterAttackTrigger leftAttack;
    [SerializeField] private CharacterAttackTrigger rightAttack;
    [SerializeField] private CharacterAttackTrigger upAttack;
    [SerializeField] private CharacterAttackTrigger downAttack;

    public void EndAttack()
    {
        switch (beforeAttackDirection)
        {
            case AttackDirection.Left:
                leftAttack.EndAnimation();
                break;
            case AttackDirection.Right:
                rightAttack.EndAnimation();
                break;
            case AttackDirection.Up:
                upAttack.EndAnimation();
                break;
            case AttackDirection.Down:
                downAttack.EndAnimation();
                break;
        }
    }

    public float PerformAttack(AttackDirection direction)
    {
        beforeAttackDirection = direction; // Store the current attack direction
        switch (direction)
        {
            case AttackDirection.Left:
                leftAttack.PerformAttack();
                break;
            case AttackDirection.Right:
                rightAttack.PerformAttack();
                break;
            case AttackDirection.Up:
                upAttack.PerformAttack();
                break;
            case AttackDirection.Down:
                downAttack.PerformAttack();
                break;
        }
        return attackAnimationDuration; // Return the duration of the attack animation
    }
}
