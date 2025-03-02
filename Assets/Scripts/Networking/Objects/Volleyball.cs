using UnityEngine;
using Fusion;
using System;

public class Volleyball : NetworkBehaviour
{
    [SerializeField] float Impulse = 6;
    [SerializeField] float impulseDelay = 0.1f;
    [SerializeField] float despawnDelay = 5.0f;

    [Networked] private TickTimer life { get; set; }

    [Networked] private TickTimer delay { get; set; }

    //Private
    private Transform CourtCenter;
    private Vector3 forward;

    private bool isGrounded;


    void Awake()
    {
        CourtCenter = Provider.Instance.CourtCenter;

    }

    private void HandleImpulseOnPress(NetworkInputData data)
    {
        if (isGrounded)
            return;

        if (data.direction.sqrMagnitude > 0)
            forward = data.direction;

        if (HasStateAuthority)
        {
            if (data.buttons.IsSet(NetworkInputData.BUTTON_0_FIRE) && delay.ExpiredOrNotRunning(Runner))
            {
                delay = TickTimer.CreateFromSeconds(Runner, impulseDelay);

                this.transform.position = transform.position + (forward.normalized * 0.05f);
                ApplyImpulse();
            }

        }

    }

    public void ApplyImpulse()
    {
        if (isGrounded)
            return;

        GetComponent<Rigidbody>().velocity = (CourtCenter.position - this.transform.position).normalized * Impulse;

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

    public void StopMoving()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        life = TickTimer.CreateFromSeconds(Runner, despawnDelay);

    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            HandleImpulseOnPress(data);

        }

        if (life.Expired(Runner))
            Runner.Despawn(Object);

    }

    public void HandleGroundTouch(Team scoringTeam)
    {
        isGrounded = true;
        Provider.Instance.GameState.IncreaseScoreFor(scoringTeam);
        StopMoving();

    }
}