using UnityEngine;

namespace ImmortalSuffering
{
    /// <summary>
    /// 플레이어를 붙잡았을 때 적용되는 능력치 저하를 처리하는 컴포넌트.
    /// [주의] 현재는 CharacterMovement에 강하게 의존하고, 여러 객체 간 Race가 있음.
    /// </summary>
    public class MovementDebuff : MonoBehaviour
    {
        [SerializeField] private CharacterMovement targetCharacterMovement;

        [System.Serializable]
        public class MovementStat
        {
            public float moveSpeed, jumpSpeed, doubleJumpSpeed;

            public void LoadFrom(CharacterMovement target)
            {
                moveSpeed = target.moveSpeed;
                jumpSpeed = target.jumpVelocity;
                doubleJumpSpeed = target.doubleJumpVelocity;
            }

            public void ApplyTo(CharacterMovement target)
            {
                target.moveSpeed = moveSpeed;
                target.jumpVelocity = jumpSpeed;
                target.doubleJumpVelocity = doubleJumpSpeed;
            }
        }

        [SerializeField] private MovementStat statOnDebuff;
        private MovementStat originalStat = new();

        void Start()
        {
            originalStat.LoadFrom(targetCharacterMovement);
        }

        public void ApplyDebuff()
        {
            originalStat.LoadFrom(targetCharacterMovement);
            statOnDebuff.ApplyTo(targetCharacterMovement);
        }

        public void RestoreStat()
        {
            originalStat.ApplyTo(targetCharacterMovement);
        }
    }
}