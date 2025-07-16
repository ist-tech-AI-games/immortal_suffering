using UnityEngine;
using UnityEngine.Events;

class CharacterHit : AHitTrigger
{
    [SerializeField] private CharacterMovement characterMovement; // Reference to the CharacterMovement component
    // TODO: 확장성을 위해, 적절한 인수를 추가할 것. 구조체를 만드는 것도 좋을 듯.
    [SerializeField] private UnityEvent onHit;

    private void Start()
    {
        characterMovement = GetComponent<CharacterMovement>();
    }

    public override void OnHit(float damage, float knockbackRatio, Transform attacker)
    {
        if (characterMovement)
        {
            Debug.Log($"Character {characterMovement.name} hit by {attacker.name} with damage: {damage}, knockback ratio: {knockbackRatio}");
            characterMovement.CharacterAttackedTriggered(damage, knockbackRatio, attacker);
            onHit?.Invoke();
        }
    }
}