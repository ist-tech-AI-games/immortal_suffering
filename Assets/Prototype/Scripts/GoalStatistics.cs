using TMPro;
using UnityEngine;

namespace ImmortalSuffering
{
    /// <summary>
    /// 결과 화면에서 걸린 시간 등 이번 실험에서의 통계를 보여 주는 컴포넌트.
    /// 나중에 추가할 데이터가 있다면 여기를 수정할 것.
    /// </summary>
    public class GoalStatistics : MonoBehaviour
    {
        [Header("Output")]
        [SerializeField] private TextMeshProUGUI statTextComp;
        [Multiline, SerializeField] private string placeholder = @"Elapsed Time: {0:0.00}s
Final Damage: {1:0.0}%";

        [Header("Sources")]
        [SerializeField] private StageTimer stageTimer;
        [SerializeField] private CharacterMovement characterMovement;

        public void UpdateUI()
        {
            statTextComp.SetText(placeholder, stageTimer.CurrentTime, characterMovement.damageGot);
        }
    }
}