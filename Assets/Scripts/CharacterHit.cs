using Unity.VisualScripting;
using UnityEngine;

class CharacterHit : AHitTrigger
{
    [SerializeField] private CharacterMovement characterMovement; // Reference to the CharacterMovement component

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
        }
    }
}