using UniRx;
using UnityEngine;

public class BallHitting : MonoBehaviour, IPlayer
{
    [Header("Player Attributes")]
    [SerializeField] float maxDistanceFromBall = 3;

    public Team Team { get; private set; }
    public bool IsAI { get; private set; }

    private Vector3 forward;
    private bool isTouchingVolleyball;
    private float currentDistanceFromBall;
    private LocalVolleyball volleyball;
    private VolleyballHitTrigger possibleBallTrigger;
    private JoystickSingleplayer joystickSingleplayer;
    private bool isInitialized;
    private bool ballHitQueued;


    public void Initialize(Team team, bool isAI)
    {

        joystickSingleplayer = GetComponent<JoystickSingleplayer>();
        joystickSingleplayer.Initialize(team);

        GetComponent<PlayerMovement>().Initialize();
        GetComponent<JumpComponent>().Initialize();

        this.forward = gameObject.transform.forward;
        this.Team = team;
        this.IsAI = isAI;

        isInitialized = true;

    }

    public void InjectVolleyball(IVolleyball volleyball)
    {
        this.volleyball = (LocalVolleyball)volleyball;

    }

    public void HitTheBall()
    {
        ballHitQueued = true;

    }

    void Update()
    {
        if (!isInitialized)
            return;

        if (ballHitQueued)
        {
            Debug.Log("[BallHitting] ball Hit Queued");
            AttemptImpulseOnBall();
            ballHitQueued = false;

        }

    }

    void LateUpdate()
    {
        if (isTouchingVolleyball && (volleyball == null || !IsVolleyballWithinReach()))
        {
            Debug.Log("[BallHitting] Player NOT TOUCHING BALL ANYMORE ===========================");
            isTouchingVolleyball = false;
        }

    }

    private bool AttemptImpulseOnBall()
    {

        Debug.Log($"[BallHitting] Attempting ball impulse...");

        if (volleyball == null)
        {
            Debug.Log($"[BallHitting] Volleyball is NULL - ABORT.");
            return false;
        }

        if (IsWithinHittingDistance())
        {
            Debug.Log($"[BallHitting] Applied impulse to {volleyball.name}");
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
            Debug.Log("[BallHitting] Is touching volleyball (trigger)");
            return true;
        }

        if (IsVolleyballWithinReach())
        {
            Debug.Log("[BallHitting] Is within hitting distance (Vector3.Distance).");
            return true;
        }

        Debug.Log($"[BallHitting] Not able to apply ball impulse yet.");

        return false;

    }

    private bool IsVolleyballWithinReach()
    {
        currentDistanceFromBall = Vector3.Distance(this.transform.position, volleyball.transform.position);

        //Debug.LogWarning($"Distance {distanceFromBall} smaller than {maxImpulseDistance}? {distanceFromBall <= maxImpulseDistance}");

        return currentDistanceFromBall <= maxDistanceFromBall;

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

    private void OnTriggerEnter(Collider other)
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
            Debug.Log($"[BallHitting] On Trigger Enter Ball ({other.name}) ----------------------");
        }

    }

    private void OnTriggerStay(Collider other)
    {
        possibleBallTrigger = other.GetComponent<VolleyballHitTrigger>();
        SetVolleyballTouching(possibleBallTrigger);

    }

}