using Fusion;
using Sirenix.OdinInspector;
using System;
using UniRx;
using UnityEngine;

[RequireComponent(typeof(JumpComponent), typeof(JoystickSingleplayer), typeof(SinglePlayerMovement))]
public class PlayerLocal : MonoBehaviour
{
    [Header("Player Attributes")]
    [SerializeField] float maxDistanceFromBall = 3;

    public Team Team { get; private set; }

    private bool isAI;
    private Vector3 forward;
    private bool isTouchingVolleyball;
    private float currentDistanceFromBall;
    private Volleyball volleyball;
    private JumpComponent jumpComponent;
    private VolleyballHitTrigger possibleBallTrigger;
    private JoystickSingleplayer joystickSingleplayer;
    private SinglePlayerMovement singlePlayerMovement;
    private bool isInitialized;

    public void Initialize(Team team, bool isAI)
    {
        forward = Vector3.forward;

        joystickSingleplayer = GetComponent<JoystickSingleplayer>();
        joystickSingleplayer.Initialize(team);

        singlePlayerMovement = GetComponent<SinglePlayerMovement>();
        singlePlayerMovement.Initialize();

        jumpComponent = GetComponent<JumpComponent>();
        jumpComponent.Initialize();

        this.Team = team;
        this.isAI = isAI;

        isInitialized = true;

    }

    void Update()
    {
        if (!isInitialized)
            return;

        if (joystickSingleplayer.GetFireButtonDown())
        {
            Debug.Log("[Ball-Player] BUTTON_0_FIRE Pressed");
            AttemptImpulseOnBall();

        }

        if (isTouchingVolleyball && (volleyball == null || !IsVolleyballWithinReach()))
        {
            Debug.Log("[Ball-Player] Player NOT TOUCHING BALL ANYMORE ===========================");
            isTouchingVolleyball = false;
        }

    }

    private bool AttemptImpulseOnBall()
    {

        Debug.Log($"[Ball-P] Attempting ball impulse...");

        if (volleyball == null)
        {
            Debug.Log($"[Ball-P] Volleyball is NULL - ABORT.");
            return false;
        }

        if (IsWithinHittingDistance())
        {
            Debug.Log($"[Ball-P] Applied impulse to {volleyball.name}");
            volleyball.ApplyImpulse(this.transform.forward, this.transform.forward);
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

        return currentDistanceFromBall <= maxDistanceFromBall;

    }

    public void OnTriggerEnter(Collider other)
    {
        possibleBallTrigger = other.GetComponent<VolleyballHitTrigger>();

        if (possibleBallTrigger == null)
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
            if (!trigger.LocalVolleybal.IsGrounded)
            {
                isTouchingVolleyball = true;
                InjectVolleyball(trigger.LocalVolleybal);
                return true;
            }

        }
        return false;
    }

    public void InjectVolleyball(Volleyball volleyball)
    {
        this.volleyball = volleyball;

    }

}