
using System;
using UnityEngine;

public class AIMovement : BaseMovement
{

    private IVolleyball currentVolleyball;

    private Vector3 auxPosition;
    private Vector3 auxDistance;

    public void ForceStop()
    {
        Debug.Log("[AI] STOP (SO WE CAN HIT THE BALL)");
        moveDirection = Vector3.zero;
        FaceOtherCourtImmediately();
    }

    public void UpdateMovementDirection(bool isBallInSight)
    {
        if (currentVolleyball == null)
        {
            Debug.Log("[AI] NO VOLLEYBALL");
            ForceStop();
            return;
        }

#if UNITY_EDITOR
        PrintSituation(isBallInSight);
#endif

        if (isBallInSight)
        {
            auxDistance = GetAdaptedVolleyballPosition() - this.transform.position;
            moveDirection = auxDistance.normalized;
            moveDirection.y = 0;
        }
        else
        {
            auxDistance = GetAdaptedSpawnCenter() - this.transform.position;
            moveDirection = auxDistance.normalized;
            moveDirection.y = 0;
        }

        if (auxDistance.sqrMagnitude <= 0.02f)
        {
            moveDirection = Vector3.zero;
            Debug.Log("[AI] CLOSE ENOUGH, NOW STOP");
        }

    }

    private void PrintSituation(bool isBallInSight)
    {

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

    private Vector3 GetAdaptedSpawnCenter()
    {
        auxPosition = spawnCenter;
        auxPosition.y = this.transform.position.y;
        return auxPosition;

    }

    private Vector3 GetAdaptedVolleyballPosition()
    {
        auxPosition = currentVolleyball.Position;
        auxPosition.y = this.transform.position.y;
        return auxPosition;

    }

}