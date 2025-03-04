using Fusion;
using Sirenix.OdinInspector;
using System;
using UniRx;
using UnityEngine;

[RequireComponent(typeof(NetworkJumpComponent))]
public class Player : NetworkBehaviour
{
    [Header("Player Attributes")]
    [SerializeField] float maxImpulseDistance = 3;
    [SerializeField] float timeBetweenBufferAttempts;

    public Team Team { get; private set; }

    //Private
    private NetworkCharacterController netCharController;
    private Vector3 forward;
    private NetworkJumpComponent jumpComponent;
    private NetworkVolleyball volleyball;
    private bool isTouchingVolleyball;
    private VolleyballHitTrigger possibleBallTrigger;
    private int bufferedBallBounce = 0;
    private float previousAttemptTime;
    private float currentDistanceFromBall;
    private bool isAI;

    void Awake()
    {
        netCharController = GetComponent<NetworkCharacterController>();
        jumpComponent = GetComponent<NetworkJumpComponent>();
        forward = Vector3.forward;

    }

    public void Initialize(Team team, bool isAI)
    {
        this.Team = team;
        this.isAI = isAI;

    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            MoveNetworkCharacter(data.direction);
            HandleNetworkButtonPress(data);

        }

    }

    private void HandleNetworkButtonPress(NetworkInputData data)
    {
        if (HasStateAuthority)
        {
            HandleButtonPresses(data.direction, data.buttons.IsSet(NetworkInputData.BUTTON_0_FIRE), data.buttons.IsSet(NetworkInputData.BUTTON_1_JUMP));

        }

    }

    private void HandleButtonPresses(Vector3 direction, bool fireButton, bool jumpButton)
    {
        if (direction.sqrMagnitude > 0)
            this.forward = direction;

        if (jumpButton)
        {
            jumpComponent.Jump();

        }

        if (fireButton)
        {
            Debug.Log("[Ball-Player] BUTTON_0_FIRE Pressed");
            if (!AttemptBallBounce())
            {
                BufferBallBounceAttempts();

            }

        }

        if ((bufferedBallBounce > 0) && IsTimeForBufferedBounce())
        {
            Debug.Log($"[Ball-Player] Executing buffered ball bounce {bufferedBallBounce}");
            if (!AttemptBallBounce())
            {
                bufferedBallBounce--;
            }

        }

        if (isTouchingVolleyball && (volleyball == null || !IsVolleyballWithinReach()))
        {
            Debug.Log("[Ball-Player] Player NOT TOUCHING BALL ANYMORE ===========================");
            isTouchingVolleyball = false;
        }

    }

    private bool IsTimeForBufferedBounce()
    {
        return (Time.time - previousAttemptTime) >= timeBetweenBufferAttempts;

    }

    private void BufferBallBounceAttempts()
    {
        bufferedBallBounce = 3;

    }

    private bool AttemptBallBounce()
    {
        previousAttemptTime = Time.time;

        Debug.Log($"[Ball-P] Attempting ball impulse...");

        if(volleyball == null)
        {
            Debug.Log($"[Ball-P] Volleyball is NULL - ABORT.");
            return false;
        }

        if (IsWithinHittingDistance())
        {
            Debug.Log($"[Ball-P] Applied impulse to {volleyball.name}");
            volleyball.ApplyImpulse(this.transform.forward, this.transform.forward);
            bufferedBallBounce = 0;
            isTouchingVolleyball = false;
            return true;

        }

        return false;

    }

    private bool IsWithinHittingDistance()
    {
        if (isTouchingVolleyball)
        {
            Debug.Log("[Ball-Player] Is touching volleyball (trigger)");
            return true;
        }

        if (IsVolleyballWithinReach())
        {
            Debug.Log("[Ball-Player] Is within hitting distance (Vector3.Distance).");
            return true;
        }

        Debug.Log($"[Ball-P] Not able to apply ball impulse yet.");

        return false;

    }

    private bool IsVolleyballWithinReach()
    {
        currentDistanceFromBall = Vector3.Distance(this.transform.position, volleyball.transform.position);

        //Debug.LogWarning($"Distance {distanceFromBall} smaller than {maxImpulseDistance}? {distanceFromBall <= maxImpulseDistance}");

        return currentDistanceFromBall <= maxImpulseDistance;

    }

    private void MoveNetworkCharacter(Vector3 direction)
    {
        direction.Normalize();
        netCharController.Move(direction * Runner.DeltaTime);

    }

    public void OnTriggerEnter(Collider other)
    {
        possibleBallTrigger = other.GetComponent<VolleyballHitTrigger>();

        if(possibleBallTrigger == null)
        {
            return;
        }

        if (isTouchingVolleyball && (possibleBallTrigger != null) && (this.volleyball != null) && (this.volleyball == possibleBallTrigger.Volleyball))
            return;

        if (SetVolleyballTouching(possibleBallTrigger))
        {
            Debug.Log($"[Player-Ball] On Trigger Enter Ball ({other.name}) ----------------------");
        }

    }

    public void OnTriggerStay(Collider other)
    {
        possibleBallTrigger = other.GetComponent<VolleyballHitTrigger>();
        SetVolleyballTouching(possibleBallTrigger);

    }

    private bool SetVolleyballTouching(VolleyballHitTrigger trigger)
    {
        if (trigger != null)
        {
            if (!trigger.Volleyball.IsGrounded)
            {
                isTouchingVolleyball = true;
                InjectVolleyball(trigger.Volleyball);
                return true;
            }

        }
        return false;
    }

    public void InjectVolleyball(NetworkVolleyball volleyball)
    {
        this.volleyball = volleyball;

    }

}