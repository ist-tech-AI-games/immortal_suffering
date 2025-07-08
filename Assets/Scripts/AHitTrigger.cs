using UnityEngine;

public abstract class AHitTrigger : MonoBehaviour
{
    // This class serves as a base class for hit triggers in the game.
    // It can be extended to implement specific hit trigger behaviors.
    
    // This method should be overridden by derived classes to define the hit behavior.
    public abstract void OnHit(float damage, Transform attacker);
}