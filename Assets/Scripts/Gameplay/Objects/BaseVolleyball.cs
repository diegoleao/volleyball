using System;
using UniRx;
using UnityEngine;

public abstract class BaseVolleyball : MonoBehaviour
{

    [Header("Parameters")]
    [SerializeField] float groundTouchDelay = 0.3f;
    [SerializeField] float Impulse = 13;
    [SerializeField] float despawnDelay = 1;
    [SerializeField] float spawnHeight = 5;

    [Header("References")]
    [SerializeField] SphereCollider proximityTrigger;
    [SerializeField] GroundProjection IndicatorCircle;
    public string Name => gameObject.name;
    public float SpawnHeight => spawnHeight;

    public Vector3 Position
    {
        get
        {
            if (this == null || transform == null) return Vector3.zero;

            return transform.position;

        }
    }

    public bool IsGrounded { get; private set; }

    public bool IsGroundChecking
    {
        get
        {
            return IsGrounded || bufferedGrounded;
        }
    }

    //Private
    private Vector3 courtCenter;
    private Vector3 forward;
    private Rigidbody rb;
    private static int idCounter;
    private bool bufferedGrounded;
    private Vector3 centerDirection;

    void Awake()
    {
        ApplyDebugChanges();
        courtCenter = Provider.Instance.CourtCenter.position;
        rb = GetComponent<Rigidbody>();
        SetDescriptiveName();
        Register(this);

    }

    public abstract void Register(BaseVolleyball volleyball);

    public abstract void IncreaseScoreForTeam(Team scoringTeam);

    private void ApplyDebugChanges()
    {
#if UNITY_EDITOR
        if (Provider.Instance.SpeedUpForDebugging)
        {
            despawnDelay = 0;
        }
#endif

    }

    public void ApplyImpulse(Vector3 playerDirection)
    {
        if (IsGrounded)
        {
            Debug.Log($"Ignoring hit on grounded Volleybal {this?.name}");
            return;
        }

        bufferedGrounded = false;

        playerDirection = ComposeFinalHitDirection(playerDirection);

        rb.velocity = forward.normalized * Impulse;

        Debug.Log($"Hitting ball {idCounter} forward ({forward.normalized}) with Velocity {rb.velocity}");

    }

    private Vector3 ComposeFinalHitDirection(Vector3 playerDirection)
    {
        centerDirection = (courtCenter - this.transform.position).normalized;

        playerDirection = playerDirection.normalized;

        playerDirection.y = centerDirection.y;

        forward = centerDirection + playerDirection;

        return playerDirection;

    }

    public async void StopMoving()
    {
        if (rb)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        IndicatorCircle.StopAnimation();
        proximityTrigger.GetComponentInChildren<SphereCollider>().enabled = false;

        await Observable.Timer(TimeSpan.FromSeconds(this.despawnDelay));

        if (this != null && this.gameObject != null) Destroy(this.gameObject);

    }

    public async void HandleGroundTouch(Team scoringTeam)
    {
        bufferedGrounded = true;
        Debug.Log($"[Ball-Floor] ({this.name}) Buffered Ground");

        await Observable.Timer(TimeSpan.FromSeconds(this.groundTouchDelay));

        if (bufferedGrounded)
        {
            Debug.Log($"[Ball-Floor] ({this.name}) Still grounded. CONFIRM SCORE!");
            IsGrounded = true;
            if (proximityTrigger) proximityTrigger.enabled = false;
            IncreaseScoreForTeam(scoringTeam);
            StopMoving();

        }
        else
        {
            Debug.Log("[Ball-Floor] Not grounded anymore. IGNORING...");
        }

    }

    private void SetDescriptiveName()
    {
        idCounter++;
        this.gameObject.name += $" [{idCounter}]";
    }

}