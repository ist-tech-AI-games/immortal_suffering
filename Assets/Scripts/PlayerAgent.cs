using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.VisualScripting;
using System.CodeDom.Compiler;
using ImmortalSuffering;

public class PlayerAgent : Agent
{
    [Header("Input")]
    [SerializeField] private bool[] moveInput = new bool[8];

    [Header("Components")]
    [SerializeField] private CharacterMovement characterMovement;
    [SerializeField] private Rigidbody2D characterRigidbody2D;
    [SerializeField] private Transform goalTransform;
    [SerializeField] private DistanceCalculator distanceCalculator;
    [SerializeField] private StageTimer stageTimer;

    [Header("Referencing Game Objects")]
    [SerializeField] private GameObject enemyParent;
    [SerializeField] private GameObject SpawnPoint;
    private int maxEnemyCount = 10;
    private Vector2 goalPosition;
    private Vector2 emptyVector2D;
    private int i = 0;
    private bool isInitialized = false;

    public override void OnEpisodeBegin()
    {
        moveInput = new bool[8] { false, false, false, false, false, false, false, false };
        goalPosition = goalTransform.position;

        emptyVector2D = new Vector2(0f, 0f);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // O padding for first frame issue
        if (!isInitialized)
        {
            float[] playerPadding = new float[13];
            sensor.AddObservation(playerPadding);
            for (int i = 0; i < maxEnemyCount; i++)
            {
                float[] enemyPadding = new float[10];
                sensor.AddObservation(enemyPadding);
            }
            Debug.Log($"Observations Collected: {sensor.ObservationSize()}, Enemy Count: {enemyParent.transform.childCount}/{maxEnemyCount}");
            Debug.Log("Zero Padding Applied for Initialization");

            return;
        }

        Vector2 playerPosition = transform.position;
        Vector2 playerVelocity = characterRigidbody2D.linearVelocity;
        float playerCumulatedDamage = characterMovement.damageGot;
        bool isActionable = characterMovement.currentState != PlayerState.AttackedAndStunned && characterMovement.currentState != PlayerState.Grabbed;
        bool isHit = characterMovement.currentState == PlayerState.AttackedAndStunned || characterMovement.currentState == PlayerState.Grabbed;
        bool isDobbleJumpAvailable = characterMovement.currentState == PlayerState.Jumping;
        bool isAttackable = characterMovement.currentState != PlayerState.AttackedAndStunned;
        // Vector2 goalPosition is already set in OnEpisodeBegin

        float goalPlayerDistance = distanceCalculator.Distance;
        float timeElapsed = stageTimer.CurrentTime;
        // Minimap Texture is added in Rendered Sensor Component

        // Add Player Observations
        sensor.AddObservation(playerPosition);
        sensor.AddObservation(playerVelocity);
        sensor.AddObservation(playerCumulatedDamage);
        sensor.AddObservation(isActionable);
        sensor.AddObservation(isHit);
        sensor.AddObservation(isDobbleJumpAvailable);
        sensor.AddObservation(isAttackable);
        sensor.AddObservation(goalPosition);
        sensor.AddObservation(goalPlayerDistance);
        sensor.AddObservation(timeElapsed);

        // Add enemy Observations
        for (int i = 0; i < enemyParent.transform.childCount; i++)
        {
            Transform enemyTransform = enemyParent.transform.GetChild(i);
            EnemyType enemyType = enemyTransform.GetComponent<EnemyInfo>().EnemyType;
            Vector2 enemyPosition = enemyTransform.position;
            Vector2 enemyVelocity = new Vector2(0f, 0f);
            float enemyHealth = 0f;
            bool currentState = false;
            if (enemyType == EnemyType.Skeleton)
            {
                enemyVelocity = enemyTransform.GetComponent<Rigidbody2D>().linearVelocity;
                enemyHealth = enemyTransform.GetComponent<EnemyHitTrigger>().RemainingHealth;
                currentState = enemyTransform.GetChild(5).GetComponent<GrabAgent>().IsGrabbing;
            }
            else if (enemyType == EnemyType.BombKid)
            {
                enemyVelocity = enemyTransform.GetComponent<Rigidbody2D>().linearVelocity;
                enemyHealth = 1f;
                currentState = enemyTransform.GetChild(4).GetComponent<TriggerDetector>().IsExploding;
            }
            else if (enemyType == EnemyType.Turret)
            {
                enemyHealth = enemyTransform.GetChild(3).transform.GetComponent<EnemyHitTrigger>().RemainingHealth;
                currentState = false;
            }
            else
            {
                Debug.LogError("Enemy Type is not set properly.");
            }
            sensor.AddOneHotObservation((int)enemyType, 4);
            sensor.AddObservation(enemyPosition);
            sensor.AddObservation(enemyVelocity);
            sensor.AddObservation(enemyHealth);
            sensor.AddObservation(currentState);
        }

        // Padding Blank Enemies
        for (int i = enemyParent.transform.childCount; i < maxEnemyCount; i++)
        {
            sensor.AddOneHotObservation((int)EnemyType.None, 4);
            sensor.AddObservation(emptyVector2D);
            sensor.AddObservation(emptyVector2D);
            sensor.AddObservation(0f);
            sensor.AddObservation(false);
        }

        Debug.Log($"Observations Collected: {sensor.ObservationSize()}, Enemy Count: {enemyParent.transform.childCount}/{maxEnemyCount}");
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        if (!Academy.Instance.IsCommunicatorOn)
        {
            return;
        }

        var da = actionBuffers.DiscreteActions;

        bool w = da[0] == 1;
        bool a = da[1] == 1;
        bool s = da[2] == 1;
        bool d = da[3] == 1;

        // Handle conflicting inputs
        bool filteredW = (w != s) && w;
        bool filteredS = (w != s) && s;
        bool filteredA = (a != d) && a;
        bool filteredD = (a != d) && d;

        bool[] filteredMoveInput = new bool[4];
        filteredMoveInput[0] = filteredW;
        filteredMoveInput[1] = filteredA;
        filteredMoveInput[2] = filteredS;
        filteredMoveInput[3] = filteredD;

        if (filteredA || filteredD || filteredW || filteredS)
        {

            characterMovement.Move(moveInput);
        }

        bool up = da[4] == 1;
        bool left = da[5] == 1;
        bool down = da[6] == 1;
        bool right = da[7] == 1;


        bool filteredUp = (up && !left && !down && !right);
        bool filteredLeft = (!up && left && !down && !right);
        bool filteredDown = (!up && !left && down && !right);
        bool filteredRight = (!up && !left && !down && right);
        if (filteredUp || filteredLeft || filteredDown || filteredRight)
        {

            AttackDirection direction = AttackDirection.Up;
            if (filteredUp) direction = AttackDirection.Up;
            else if (filteredLeft) direction = AttackDirection.Left;
            else if (filteredDown) direction = AttackDirection.Down;
            else if (filteredRight) direction = AttackDirection.Right;

            characterMovement.Attack(direction);
        }
    }

    public void OnGoalReached()
    {
        SetReward(1.0f);
        Debug.Log("Goal Reached");
        EndEpisode();
    }

    public void OnEnemyGenerated(int enemyCount)
    {
        maxEnemyCount = enemyCount;
        isInitialized = true;
        Debug.Log($"Enemy Generated with Count: {enemyCount}");
    }
}
