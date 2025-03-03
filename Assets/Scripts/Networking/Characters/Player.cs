using Fusion;
using System;
using UniRx;
using UnityEngine;

[RequireComponent(typeof(JumpComponent))]
public class Player : NetworkBehaviour
{
    [Header("Player Attributes")]
    [SerializeField] float maxImpulseDistance = 0.5f;
    [SerializeField] float timeBetweenBufferAttempts;

    //Private
    private NetworkCharacterController netCharController;
    private Vector3 forward;
    private JumpComponent jumpComponent;
    private Volleyball volleyball;
    private bool isTouchingVolleyball;
    private VolleyballHitTrigger possibleBallTrigger;
    private int bufferedBallBounce = 0;
    private float previousAttemptTime;


    void Awake()
    {
        netCharController = GetComponent<NetworkCharacterController>();
        jumpComponent = GetComponent<JumpComponent>();
        forward = Vector3.forward;

    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            MoveCharacter(data);
            HandleButtonPress(data);
        }

    }

    private void HandleButtonPress(NetworkInputData data)
    {
        if (data.direction.sqrMagnitude > 0)
            forward = data.direction;

        if (HasStateAuthority)
        {
            if (data.buttons.IsSet(NetworkInputData.BUTTON_1_JUMP))
            {
                jumpComponent.Jump();

            }

            if (data.buttons.IsSet(NetworkInputData.BUTTON_0_FIRE))
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

            if (volleyball == null || !IsVolleyballWithinReach())
            {
                Debug.Log("[Ball-Player] Player NOT TOUCHING BALL ANYMORE ===========================");
                isTouchingVolleyball = false;
            }

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
        return Vector3.Distance(this.transform.position, volleyball.transform.position) <= maxImpulseDistance;
    }

    private void MoveCharacter(NetworkInputData data)
    {
        data.direction.Normalize();

        netCharController.Move(data.direction * Runner.DeltaTime);

    }

    public void OnTriggerEnter(Collider other)
    {
        possibleBallTrigger = other.GetComponent<VolleyballHitTrigger>();
        if (possibleBallTrigger != null)
        {
            if (!possibleBallTrigger.Volleyball.IsGrounded)
            {
                isTouchingVolleyball = true;
                InjectVolleyball(possibleBallTrigger.Volleyball);
                Debug.Log($"[Player-Ball] On Trigger Enter Ball ({other.name}) ===========================");
            }

        }

    }

    public void InjectVolleyball(Volleyball volleyball)
    {
        this.volleyball = volleyball;

    }

}