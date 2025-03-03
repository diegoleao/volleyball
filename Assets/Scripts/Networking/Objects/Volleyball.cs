using UnityEngine;
using Fusion;
using System;
using UniRx;

public class Volleyball : NetworkBehaviour
{
    [Header("Parameters")]
    [SerializeField] float groundTouchDelay = 0.3f;
    [SerializeField] float Impulse = 6;
    [SerializeField] float despawnDelay = 5.0f;

    [Header("References")]
    [SerializeField] SphereCollider proximityTrigger;

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


    void Awake()
    {
        CourtCenter = Provider.Instance.CourtCenter.position;
        rb = GetComponent<Rigidbody>();
        Provider.Register<Volleyball>(this);
        idCounter++;
        this.gameObject.name += " "+idCounter;

    }

    public void ApplyImpulse(Vector3 hitDirection, Vector3 playerDirection)
    {
        if (IsGrounded)
        {
            Debug.Log($"Ignoring hit on grounded Volleybal {this?.name}");
            return;
        }

        bufferedGrounded = false;

        //forward = hitDirection.sqrMagnitude > 0 ? hitDirection : playerDirection;
        forward = (CourtCenter - this.transform.position);
        rb.velocity = forward.normalized * Impulse;

        Debug.Log($"Hitting ball {idCounter} forward ({forward.normalized}) with Velocity {rb.velocity}");

    }

    //private Vector3 PredictPosition(Vector3 initialPosition, Vector3 initialVelocity, Vector3 impulse, float mass, float time)
    //{
    //    if (mass <= 0)
    //    {
    //        Debug.LogError("Mass must be greater than zero.");
    //        return initialPosition;
    //    }

    //    // Compute final velocity after impulse
    //    Vector3 finalVelocity = initialVelocity + (impulse / mass);

    //    // Predict future position using kinematic equation: x = x0 + v*t + 0.5*a*t^2
    //    Vector3 acceleration = impulse / mass; // Assuming impulse is the only force
    //    Vector3 futurePosition = initialPosition + finalVelocity * time + 0.5f * acceleration * time * time;

    //    return futurePosition;

    //}

    public async void StopMoving()
    {
        if (rb)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        await Observable.Timer(TimeSpan.FromSeconds(this.despawnDelay));

        if(this != null && this.gameObject != null) Destroy(this.gameObject);

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
            if(proximityTrigger) proximityTrigger.enabled = false;
            Provider.Instance.GameState.IncreaseScoreFor(scoringTeam);
            StopMoving();

        }
        else
        {
            Debug.Log("[Ball-Floor] Not grounded anymore. IGNORING...");
        }

    }

    ////Assigned in Inspector through VolleyballHitTrigger component's event.
    //public void HitTriggerEntered(Collider other)
    //{
    //    if (this.IsGrounded)
    //        return;

    //    other.GetComponent<Player>().SetVolleyballColliding(this, true);

    //}

    ////Assigned in Inspector through VolleyballHitTrigger component's event.
    //public void HitTriggerLeft(Collider other)
    //{
    //    if (this.IsGrounded)
    //        return;

    //    other.GetComponent<Player>().SetVolleyballColliding(this, false);

    //}

}