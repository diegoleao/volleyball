
using System;
using UnityEngine;

public abstract class BaseBallImpulse : MonoBehaviour
{
    [Header("Player Attributes")]
    [SerializeField] float maxDistanceFromBall = 3;

    public Team Team { get; private set; }

    protected bool isTouchingVolleyball;
    protected BaseVolleyball volleyball;
    protected bool ballHitQueued;

    private float currentDistanceFromBall;
    private VolleyballHitTrigger possibleBallTrigger;
    private bool isInitialized;


    public void Initialize(Team team)
    {
        this.Team = team;
        isInitialized = true;

    }

    public void InjectVolleyball(BaseVolleyball volleyball)
    {
        this.volleyball = volleyball;

    }

    public void HitTheBall()
    {
        ballHitQueued = true;

    }

    void Update()
    {
        if (!isInitialized)
            return;

        UpdateImpulse();

    }

    public abstract void UpdateImpulse();

    public abstract void LateUpdateImpulse();

    void LateUpdate()
    {
        LateUpdateImpulse();

    }

    protected abstract bool AttemptImpulseOnBall();

    protected bool ApplySimpleBuiltInImpulse()
    {
        Debug.Log($"[BallHitting] Attempting ball impulse...");

        if (volleyball == null)
        {
            Debug.Log($"[BallHitting] Volleyball is NULL - ABORT.");
            return false;
        }

        if (IsWithinHittingDistance())
        {
            Debug.Log($"[BallHitting] Applied impulse to {volleyball.Name}");
            volleyball.ApplyImpulse(playerDirection: this.transform.forward);
            isTouchingVolleyball = false;
            return true;

        }

        return false;
    }


    protected bool IsWithinHittingDistance()
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

    protected bool IsVolleyballWithinReach()
    {
        currentDistanceFromBall = Vector3.Distance(this.transform.position, volleyball.Position);

        //Debug.LogWarning($"Distance {distanceFromBall} smaller than {maxImpulseDistance}? {distanceFromBall <= maxImpulseDistance}");

        return currentDistanceFromBall <= maxDistanceFromBall;

    }

    protected bool SetVolleyballTouching(VolleyballHitTrigger trigger)
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

    void OnTriggerEnter(Collider other)
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

    void OnTriggerStay(Collider other)
    {
        possibleBallTrigger = other.GetComponent<VolleyballHitTrigger>();
        SetVolleyballTouching(possibleBallTrigger);

    }

}