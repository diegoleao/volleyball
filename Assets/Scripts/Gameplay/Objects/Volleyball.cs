using System;
using UniRx;
using UnityEngine;

public class Volleyball : MonoBehaviour, IVolleyball
{
    [Header("Parameters")]
    [SerializeField] float groundTouchDelay = 0.3f;
    [SerializeField] float Impulse = 6;
    [SerializeField] float despawnDelay = 5.0f;

    [Header("References")]
    [SerializeField] SphereCollider proximityTrigger;

    [SerializeField] float spawnHeight = 5;
    public float SpawnHeight => spawnHeight;

    public bool IsGrounded { get; private set; }

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
        Provider.Register<NetworkVolleyball>(this);
        idCounter++;
        this.gameObject.name += " " + idCounter;

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

        await Observable.Timer(TimeSpan.FromSeconds(this.despawnDelay));

        if (this != null && this.gameObject != null) Destroy(this.gameObject);

    }

    public async void HandleGroundTouch(Team scoringTeam)
    {
        bufferedGrounded = true;
        Debug.Log("[Ball-Floor] Buffered Ground");

        await Observable.Timer(TimeSpan.FromSeconds(this.groundTouchDelay));

        if (bufferedGrounded)
        {
            Debug.Log("[Ball-Floor] Still grounded. CONFIRM touch!");
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

}