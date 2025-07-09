using Unity.VisualScripting;
using UnityEngine;

class CharacterHit : AHitTrigger
{
    [SerializeField] private CharacterMovement characterMovement; // Reference to the CharacterMovement component

    private void Start()
    {
        // Initialize the characterMovement component if not set
        if (characterMovement == null)
        {
            characterMovement = GetComponent<CharacterMovement>();
        }
    }

    public override void OnHit(float damage, Transform attacker)
    {
        if (characterMovement)
        {
            characterMovement.CharacterAttackedTriggered(damage, attacker);
        }
    }
}