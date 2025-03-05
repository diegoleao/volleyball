using UniRx;
using UnityEngine;

public class BallHitting : BaseBallImpulse
{

    protected override bool AttemptImpulseOnBall()
    {
        return ApplySimpleBuiltInImpulse();

    }

    public override void UpdateImpulse()
    {
        if (ballHitQueued)
        {
            Debug.Log("[BallHitting] ball Hit Queued");
            AttemptImpulseOnBall();
            ballHitQueued = false;
        }

    }

    public override void LateUpdateImpulse()
    {
        if (isTouchingVolleyball && (volleyball == null || !IsVolleyballWithinReach()))
        {
            Debug.Log("[BallHitting] Player NOT TOUCHING BALL ANYMORE ===========================");
            isTouchingVolleyball = false;
        }

    }

}