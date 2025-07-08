using UnityEngine;

public abstract class AHitTrigger : MonoBehaviour
{
    public abstract void OnHit(float damage, Transform attacker);
}