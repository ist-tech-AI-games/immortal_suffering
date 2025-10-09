using System;
using UnityEngine;

public class WallHitParticle : MonoBehaviour
{
    [SerializeField] private float destroyAfterSec;
    private void FixedUpdate()
    {
        if (destroyAfterSec < 0) Destroy(gameObject);
        else destroyAfterSec -= Time.fixedDeltaTime;
    }
}
