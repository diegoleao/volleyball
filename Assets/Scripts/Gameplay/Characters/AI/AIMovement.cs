
using System;
using UnityEngine;

public class AIMovement : BaseMovement
{
#if UNITY_EDITOR
    [SerializeField] bool IsDebugging;
#endif

    private BaseVolleyball currentVolleyball;

    private Vector3 auxPosition;
    private Vector3 auxDistance;

    public void ForceStop()
    {
        Log("STOP (SO WE CAN HIT THE BALL)");
        moveDirection = Vector3.zero;
        //FaceOtherCourtImmediately();
    }

    public void UpdateMovementDirection(bool isBallInSight)
    {
        if (currentVolleyball == null)
        {
            Log("NO VOLLEYBALL");
            ForceStop();
            return;
        }

        if (isBallInSight)
        {
            Log("MOVING TOWARDS IT");
            auxDistance = GetAdaptedVolleyballPosition() - this.transform.position;
            moveDirection = auxDistance.normalized;
            moveDirection.y = 0;
        }
        else
        {
            Log("MOVE TO CENTER, LOST BALL");
            auxDistance = GetAdaptedSpawnCenter() - this.transform.position;
            moveDirection = auxDistance.normalized;
            moveDirection.y = 0;
        }

        if (auxDistance.sqrMagnitude <= 0.02f)
        {
            moveDirection = Vector3.zero;
            Log("CLOSE ENOUGH, NOW STOP");
        }

    }

    public void InjectVolleyball(BaseVolleyball currentVolleyball)
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

    private void Log(string message)
    {
#if UNITY_EDITOR
        if (IsDebugging)
            Debug.Log($"[AI] {message}");
#endif

    }

}