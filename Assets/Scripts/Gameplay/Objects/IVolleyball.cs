
using UnityEngine;

public interface IVolleyball
{
    public bool IsGrounded { get; }

    public void HandleGroundTouch(Team scoringTeam);

    public Vector3 Position { get; }

}