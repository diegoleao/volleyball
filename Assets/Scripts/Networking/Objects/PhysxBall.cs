using UnityEngine;
using Fusion;
using System;

public class PhysxBall : NetworkBehaviour
{
    [SerializeField] float UpImpulse = 2;

    [Networked] private TickTimer life { get; set; }

    [Networked] private int lastTouchedBy  { get; set; }

    public void Init(Vector3 forward)
    {
        life = TickTimer.CreateFromSeconds(Runner, 5.0f);
        GetComponent<Rigidbody>().velocity = forward + (Vector3.up * UpImpulse);

    }

    public override void FixedUpdateNetwork()
    {
        if (life.Expired(Runner))
            Runner.Despawn(Object);

    }

    internal void StopMoving()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

    }
}