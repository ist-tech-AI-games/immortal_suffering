using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    /*
    Player 입력은 우선순위대로 처리된다
    다음 5종류의 Input이 가능하다. 
    Attack, Move Left, Move Right, Jump, Down Jump
    Attack 시 모든 Input이 무시되고, 그 프레임에는 Attack만 처리된다
    Attack이 없을 시, 3가지 Pair이 각각 독립적으로 처리되어 Character Movement에 적용된다 (WS, AD)
    단, 각 Pair은 둘 다 True라면 무시되어 Movement에 적용되지 않는다.

    총 5개의 bool을 Input으로 받는다. 
    0. Attack
    1. W : Jump
    2. A : Move Left
    3. S : Down Jump
    4. D : Move Right
    */
    [Header("Options")]
    public bool SimulateIfInputIsInputed = false;
    [Header("Flags")]
    public bool SimulateEnabled = true;
    [Header("Input")]
    [SerializeField] private bool[] moveInput = new bool[5];

    // For Unity User Simulate Test
    public void OnMove(InputValue value)
    {
        // Input 처리 - InputSystem_Actions 사용
        Vector2 vector2 = value.Get<Vector2>();
        moveInput[1] = vector2.y > 0; // W : Jump
        moveInput[2] = vector2.x < 0; // A : Move Left
        moveInput[3] = vector2.y < 0; // S : Down Jump
        moveInput[4] = vector2.x > 0; // D : Move Right
    }

    public void OnAttack(InputValue value)
    {
        // Input 처리 - InputSystem_Actions 사용
        moveInput[0] = value.isPressed; // Attack
    }

    private void FixedUpdate()
    {
        if (!SimulateEnabled)
        {
            return;
        }

        // 0: Attack, 1: W, 2: A, 3: S, 4: D
        if (moveInput[0])
        {
            Attack();
        }

        // 쌍: (W, S), (A, D)
        bool w = moveInput[1];
        bool a = moveInput[2];
        bool s = moveInput[3];
        bool d = moveInput[4];

        // (W, S) 쌍: 둘 다 true면 무시
        bool filteredW = (w != s) && w;
        bool filteredS = (w != s) && s;
        // (A, D) 쌍: 둘 다 true면 무시
        bool filteredA = (a != d) && a;
        bool filteredD = (a != d) && d;

        bool[] filteredMoveInput = new bool[4];
        filteredMoveInput[0] = filteredW;
        filteredMoveInput[1] = filteredA;
        filteredMoveInput[2] = filteredS;
        filteredMoveInput[3] = filteredD;

        if (filteredA || filteredD || filteredW || filteredS)
        {
            // 이동 처리
            Move(filteredMoveInput);
        }

        moveInput = new bool[5] { false, false, false, false, false };
    }

    private void Move(bool[] moveInput)
    {
        // 이동 처리 로직을 여기에 작성
        // 예: 애니메이션 재생, 위치 업데이트 등
        // 현재는 단순히 로그 출력
        Debug.Log($"Move Input: W={moveInput[0]}, A={moveInput[1]}, S={moveInput[2]}, D={moveInput[3]}");

        // 실제 이동 로직은 여기에 추가해야 합니다.
        // 예를 들어, Rigidbody를 사용하여 물리 기반 이동을 구현할 수 있습니다.
    }

    private void Attack()
    {
        Debug.Log("Attack performed");
        // Attack 처리 로직을 여기에 작성
        // 예: 애니메이션 재생, 공격력 적용 등
        // 현재는 단순히 로그 출력
    } 
}
