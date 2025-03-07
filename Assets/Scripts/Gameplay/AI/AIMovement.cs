
using System;
using UnityEngine;

public class AIMovement : BaseMovement
{

#if UNITY_EDITOR
    [SerializeField] bool IsDebugging;
#endif

    private LocalVolleyball currentVolleyball;

    private Vector3 auxPosition;
    private Vector3 auxDistance;

    private bool isStopped;

#if UNITY_ANDROID
    [SerializeField] float SpeedReductionOnAndroid = 0.5f;
    void Start()
    {
        this.speed = this.speed * 1-SpeedReductionOnAndroid;
    }
#endif

    public void StopToHitTheBall()
    {
        Log("STOP (SO WE CAN HIT THE BALL)");
        StopMoving();
    }

    public void MovementUpdate(bool isBallInSight)
    {

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
            Log("STOP (BALL DISTANCE TOO SMALL)");
            StopMoving();
        }
        else
        {
            isStopped = false;
        }

    }

    public void InjectVolleyball(LocalVolleyball currentVolleyball)
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

    private void StopMoving()
    {
        FaceOtherCourtImmediately();
        moveDirection = Vector3.zero;
        isStopped = true;

    }

    private void Log(string message)
    {
#if UNITY_EDITOR
        if (IsDebugging)
            Debug.Log($"[AI] {message}");
#endif

    }

}