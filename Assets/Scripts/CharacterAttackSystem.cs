using System;
using ImmortalSuffering;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterAttackSystem : MonoBehaviour
{
    private AttackDirection beforeAttackDirection;
    [SerializeField] private bool isAttacking;
    [SerializeField] private bool charFaceLeft;

    public bool CharFaceLeft
    {
        get
        {
            return charFaceLeft;
        }
        set
        {
            if (charFaceLeft != value)
            {
                setSwordIdlePosition(value);
            }
            charFaceLeft = value;
        }
    }
    [SerializeField] private float swordMovingDuration;
    [SerializeField] private float attackAnimationDuration;
    [SerializeField] private CharacterAttackTrigger leftAttack;
    [SerializeField] private CharacterAttackTrigger rightAttack;
    [SerializeField] private CharacterAttackTrigger upAttack;
    [SerializeField] private CharacterAttackTrigger downAttack;
    [SerializeField] private Animator swordAnimator;
    [SerializeField] private AnimatorParameterSetter parameterSetter;

    [Header("For Sword Moving")]
    [SerializeField] private Transform spriteTransform;
    [SerializeField] private bool swordMoving;
    private float timeRemained;
    [SerializeField] private Transform startTransform;
    [SerializeField] private Transform targetTransform;

    private void startMoving()
    {
        timeRemained = 0.0f;
        swordMoving = true;
    }

    private void setSwordIdlePosition(bool faceLeft)
    {
        var position = spriteTransform.position;
        startTransform.position = new Vector3(position.x, position.y, 0);
        deepCopyTransform(transform, targetTransform);
        targetTransform.position += new Vector3((faceLeft ? 1 : -1), 0, 0);
        startMoving();
    }

    private void deepCopyTransform(Transform source, Transform target)
    {
        var position = source.position;
        target.position = new Vector3(position.x, position.y, 0);

        Quaternion quaternion = new Quaternion();
        var rotation = source.rotation;
        quaternion.x = rotation.x;
        quaternion.y = rotation.y;
        quaternion.z = rotation.z;
        quaternion.w = rotation.w;
        target.rotation = quaternion;
    }

    private void Update()
    {
        if (!swordMoving) return;
        if (timeRemained / swordMovingDuration < 1.0f)
        {
            spriteTransform.position = Vector3.SlerpUnclamped(startTransform.position, targetTransform.position, timeRemained / swordMovingDuration);
            spriteTransform.rotation = Quaternion.SlerpUnclamped(startTransform.rotation, targetTransform.rotation, timeRemained / swordMovingDuration);
            timeRemained += Time.deltaTime;
        }
        else
        {
            swordMoving = true;
            deepCopyTransform(targetTransform, spriteTransform);
        }
    }

    public void EndAttack()
    {
        parameterSetter.SetBool(false);
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
        setSwordIdlePosition(charFaceLeft);
    }

    public float PerformAttack(AttackDirection direction)
    {
        beforeAttackDirection = direction; // Store the current attack direction
        startMoving();
        deepCopyTransform(spriteTransform, startTransform);
        parameterSetter.SetBool(true);
        switch (direction)
        {
            case AttackDirection.Left:
                leftAttack.PerformAttack();
                deepCopyTransform(leftAttack.transform, targetTransform);
                break;
            case AttackDirection.Right:
                rightAttack.PerformAttack();
                deepCopyTransform(rightAttack.transform, targetTransform);
                break;
            case AttackDirection.Up:
                upAttack.PerformAttack();
                deepCopyTransform(upAttack.transform, targetTransform);
                break;
            case AttackDirection.Down:
                downAttack.PerformAttack();
                deepCopyTransform(downAttack.transform, targetTransform);
                break;
        }
        return attackAnimationDuration; // Return the duration of the attack animation
    }
}
