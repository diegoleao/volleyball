
using System;
using UnityEngine;

public class NetworkBallHitting : BaseBallImpulse
{
    [Header("Player Attributes")]
    [SerializeField] float maxImpulseDistance = 3;
    [SerializeField] float timeBetweenBufferAttempts = 0.3f;

    private float previousAttemptTime;
    private int bufferedBallBounce;

    public void HitIt()
    {
        ballHitQueued = true;

    }

    public override void UpdateImpulse()
    {
        if (ballHitQueued)
        {
            if (!AttemptImpulseOnBall())
            {
                BufferBallBounceAttempts();

            }
            ballHitQueued = false;

        }

        if ((bufferedBallBounce > 0) && IsTimeForBufferedBounce())
        {
            Debug.Log($"[Ball-Player] Executing buffered ball bounce {bufferedBallBounce}");
            if (!AttemptImpulseOnBall())
            {
                bufferedBallBounce--;
            }

        }
    }

    public override void LateUpdateImpulse()
    {
        if (isTouchingVolleyball && (volleyball == null || !IsVolleyballWithinReach()))
        {
            Debug.Log("[Ball-Player] Player NOT TOUCHING BALL ANYMORE ===========================");
            isTouchingVolleyball = false;
        }

    }

    protected override bool AttemptImpulseOnBall()
    {
        previousAttemptTime = Time.time;

        Debug.Log($"[Ball-P] Attempting ball impulse...");

        if (volleyball == null)
        {
            Debug.Log($"[Ball-P] Volleyball is NULL - ABORT.");
            return false;
        }

        if (IsWithinHittingDistance())
        {
            Debug.Log($"[Ball-P] Applied impulse to {volleyball.Name}");
            volleyball.ApplyImpulse(playerDirection: this.transform.forward);
            bufferedBallBounce = 0;
            isTouchingVolleyball = false;
            return true;

        }

        return false;

    }

    public bool IsTimeForBufferedBounce()
    {
        return (Time.time - previousAttemptTime) >= timeBetweenBufferAttempts;

    }

    public void BufferBallBounceAttempts()
    {
        bufferedBallBounce = 3;

    }

}