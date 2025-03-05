
using System;
using UnityEngine;

public class AIMovement : BaseMovement
{

    private IVolleyball currentVolleyball;

    private Vector3 auxPosition;

    public void ForceStop()
    {
        Debug.Log("[AI] STOP (SO WE CAN HIT THE BALL)");
        moveDirection = Vector3.zero;

    }

    public void UpdateMovementDirection(bool isBallInSight)
    {
        PrintSituation(isBallInSight);

        if (isBallInSight)
        {
            moveDirection = (GetAdaptedVolleyballPosition() - this.transform.position).normalized;
            moveDirection.y = 0;
        }
        else
        {
            moveDirection = (GetAdaptedCourtCenter() - this.transform.position).normalized;
            moveDirection.y = 0;
        }

        if (moveDirection.sqrMagnitude <= 0.01f)
        {
            moveDirection = Vector3.zero;
            Debug.Log("[AI] CLOSE ENOUGH, NOW STOP");
        }

    }

    private Vector3 GetAdaptedCourtCenter()
    {
        auxPosition = courtCenter;
        auxPosition.y = this.transform.position.y;
        return auxPosition;

    }

    private Vector3 GetAdaptedVolleyballPosition()
    {
        auxPosition = currentVolleyball.Position;
        auxPosition.y = this.transform.position.y;
        return auxPosition;

    }

    private void PrintSituation(bool isBallInSight)
    {
        if(currentVolleyball == null)
        {
            Debug.Log("[AI] NO VOLLEYBALL");
            return;
        }

        if (isBallInSight)
        {
            Debug.Log("[AI] MOVING TOWARDS IT");
        }
        else
        {
            Debug.Log("[AI] MOVE TO CENTER, LOST BALL");
        }

    }

    public void InjectVolleyball(IVolleyball currentVolleyball)
    {
        this.currentVolleyball = currentVolleyball;

    }
}