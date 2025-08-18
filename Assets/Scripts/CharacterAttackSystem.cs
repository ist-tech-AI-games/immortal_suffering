using System;
using ImmortalSuffering;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterAttackSystem : MonoBehaviour
{
    private AttackDirection beforeAttackDirection;
    private Animator animator;
    [SerializeField] private bool isAttacking;
    [SerializeField] private float attackAnimationDuration;
    [SerializeField] private CharacterAttackTrigger leftAttack;
    [SerializeField] private CharacterAttackTrigger rightAttack;
    [SerializeField] private CharacterAttackTrigger upAttack;
    [SerializeField] private CharacterAttackTrigger downAttack;
    [SerializeField] private Animator swordAnimator;

    [Header("For Sword Moving")]
    [SerializeField] private Transform spriteTransform;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void EndAttack()
    {
        spriteTransform.gameObject.SetActive(false); // Hide the sprite after the attack
        switch (beforeAttackDirection)
        {
            case AttackDirection.Left:
                leftAttack.EndAttack();
                break;
            case AttackDirection.Right:
                rightAttack.EndAttack();
                break;
            case AttackDirection.Up:
                upAttack.EndAttack();
                break;
            case AttackDirection.Down:
                downAttack.EndAttack();
                break;
        }
    }

    public float PerformAttack(AttackDirection direction)
    {
        beforeAttackDirection = direction; // Store the current attack direction
        spriteTransform.gameObject.SetActive(true); // Ensure the sprite is active during the attack
        animator.Rebind();
        animator.Update(0f);
        switch (direction)
        {
            case AttackDirection.Left:
                leftAttack.PerformAttack();
                spriteTransform.position = leftAttack.transform.position;
                spriteTransform.rotation = leftAttack.transform.rotation;
                break;
            case AttackDirection.Right:
                rightAttack.PerformAttack();
                spriteTransform.position = rightAttack.transform.position;
                spriteTransform.rotation = rightAttack.transform.rotation;
                break;
            case AttackDirection.Up:
                upAttack.PerformAttack();
                spriteTransform.position = upAttack.transform.position;
                spriteTransform.rotation = upAttack.transform.rotation;
                break;
            case AttackDirection.Down:
                downAttack.PerformAttack();
                spriteTransform.position = downAttack.transform.position;
                spriteTransform.rotation = downAttack.transform.rotation;
                break;
        }
        return attackAnimationDuration; // Return the duration of the attack animation
    }
}
