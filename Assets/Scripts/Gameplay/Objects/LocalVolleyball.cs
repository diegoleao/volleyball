using System;
using UniRx;
using UnityEngine;

public class LocalVolleyball : MonoBehaviour, IVolleyball
{

    [Header("Parameters")]
    [SerializeField] float groundTouchDelay = 0.3f;
    [SerializeField] float Impulse = 13;
    [SerializeField] float despawnDelay = 1;
    [SerializeField] float spawnHeight = 5;
    public float SpawnHeight => spawnHeight;

    [Header("References")]
    [SerializeField] SphereCollider proximityTrigger;
    [SerializeField] GroundProjection IndicatorCircle;

    public bool IsGrounded { get; private set; }

    public Vector3 Position => transform.position;

    public bool IsGroundChecking
    {
        get
        {
            return IsGrounded || bufferedGrounded;
        }
    }

    //Private
    private Vector3 CourtCenter;

    private Vector3 forward;

    private Rigidbody rb;

    private static int idCounter;

    private bool bufferedGrounded;

    private Vector3 centerDirection;


    void Awake()
    {
        CourtCenter = Provider.Instance.CourtCenter.position;
        rb = GetComponent<Rigidbody>();
        SetDescriptiveName();
#if UNITY_EDITOR
        if (Provider.Instance.SpeedUpForDebugging)
        {
            despawnDelay = 0;
        }
#endif
        Provider.Register<NetworkVolleyball>(this);

    }

    public void ApplyImpulse(Vector3 hitDirection, Vector3 playerDirection)
    {
        if (IsGrounded)
        {
            Debug.Log($"Ignoring hit on grounded Volleybal {this?.name}");
            return;
        }

        bufferedGrounded = false;

        centerDirection = (CourtCenter - this.transform.position).normalized;

        playerDirection = playerDirection.normalized;

        playerDirection.y = centerDirection.y;

        forward = centerDirection + playerDirection;

        rb.velocity = forward.normalized * Impulse;

        Debug.Log($"Hitting ball {idCounter} forward ({forward.normalized}) with Velocity {rb.velocity}");

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
            Debug.Log($"[Ball-Floor] ({this.name}) Still grounded. CONFIRM touch!");
            IsGrounded = true;
            if (proximityTrigger) proximityTrigger.enabled = false;
            Provider.Instance.GameState.IncreaseScoreFor(scoringTeam);
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