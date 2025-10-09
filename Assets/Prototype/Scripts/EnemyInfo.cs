using UnityEngine;

public enum EnemyType
{
    None,
    Skeleton,
    BombKid,
    Turret
}

public class EnemyInfo : MonoBehaviour
{
    [SerializeField] private EnemyType enemyType;
    public EnemyType EnemyType { get { return enemyType; } }
}
