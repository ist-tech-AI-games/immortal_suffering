using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public enum AttackDirection
{
    Up,
    Left,
    Down,
    Right
}

public class PlayerInput : MonoBehaviour
{
    /*
    Player 입력은 우선순위대로 처리된다
    다음 8종류의 Input이 가능하다. 
    Attack, Move Left, Move Right, Jump, Down Jump, Attack Up, Attack Left, Attack Down, Attack Right
    단, 각 Pair은 둘 다 True라면 무시되어 Movement에 적용되지 않는다.

    총 8개의 bool을 Input으로 받는다. 
    0. W : Jump
    1. A : Move Left
    2. S : Down Jump
    3. D : Move Right
    4. Up : Attack Up
    5. Left : Attack Left
    6. Down : Attack Down
    7. Right : Attack Right
    */
    [Header("Options")]
    public bool SimulateIfInputIsInputed = false;
    [Header("Flags")]
    [SerializeField] private bool isUserInputed = false;
    [SerializeField] private bool SimulateEnabled = true;
    [Header("Input")]
    [SerializeField] private bool[] moveInput = new bool[8];
    [Header("Components")]
    [SerializeField] private CharacterMovement characterMovement;

    private void Awake()
    {
        // 초기화
        moveInput = new bool[8] { false, false, false, false, false, false, false, false };
    }

    // For Unity User Simulate Test
    public void OnMove(InputValue value)
    {
        // Input 처리 - InputSystem_Actions 사용
        Vector2 vector2 = value.Get<Vector2>();
        moveInput[0] = vector2.y > 0; // W : Jump
        moveInput[1] = vector2.x < 0; // A : Move Left
        moveInput[2] = vector2.y < 0; // S : Down Jump
        moveInput[3] = vector2.x > 0; // D : Move Right
        isUserInputed = true;
    }

    public void OnAttack(InputValue value)
    {
        Vector2 vector2 = value.Get<Vector2>();
        // Input 처리 - InputSystem_Actions 사용
        moveInput[4] = vector2.y > 0; // Attack Up
        moveInput[5] = vector2.x < 0; // Attack Left
        moveInput[6] = vector2.y < 0; // Attack Down
        moveInput[7] = vector2.x > 0; // Attack Right
        isUserInputed = true;
    }

    private void FixedUpdate()
    {
        if (!SimulateEnabled)
        {
            return;
        }

        // 쌍: (W, S), (A, D)
        bool w = moveInput[0];
        bool a = moveInput[1];
        bool s = moveInput[2];
        bool d = moveInput[3];

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
            characterMovement.Move(moveInput);
        }

        bool up = moveInput[4];
        bool left = moveInput[5];
        bool down = moveInput[6];
        bool right = moveInput[7];

        // 공격이 하나만 true인 경우에만 공격을 수행, up: 0, left: 1, down: 2, right: 3
        bool filteredUp = (up && !left && !down && !right);
        bool filteredLeft = (!up && left && !down && !right);
        bool filteredDown = (!up && !left && down && !right);
        bool filteredRight = (!up && !left && !down && right);
        if (filteredUp || filteredLeft || filteredDown || filteredRight)
        {
            // 공격 처리
            AttackDirection direction = AttackDirection.Up;
            if (filteredUp) direction = AttackDirection.Up;
            else if (filteredLeft) direction = AttackDirection.Left;
            else if (filteredDown) direction = AttackDirection.Down;
            else if (filteredRight) direction = AttackDirection.Right;

            characterMovement.Attack(direction);
        }

        if (isUserInputed)
        {
            isUserInputed = false;
            moveInput = new bool[8] { false, false, false, false, false, false, false, false };
        }
    }
}
