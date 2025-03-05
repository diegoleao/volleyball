
using System;
using UnityEngine;

public class AIMovement : BaseMovement
{

    private IVolleyball currentVolleyball;

    public void UpdateBallDirection(bool forceStop = false)
    {
        if (forceStop)
        {
            moveDirection = Vector3.zero;
        }
        else
        {
            moveDirection = (currentVolleyball.Position - this.transform.position).normalized;
            moveDirection.y = 0;
        }

    }

    public void InjectVolleyball(IVolleyball currentVolleyball)
    {
        this.currentVolleyball = currentVolleyball;

    }

}