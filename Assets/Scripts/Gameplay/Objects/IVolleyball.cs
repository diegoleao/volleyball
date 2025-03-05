
using UnityEngine;

public interface IVolleyball
{
    public string Name { get; }

    public bool IsGrounded { get; }

    public void HandleGroundTouch(Team scoringTeam);

    public void ApplyImpulse(Vector3 hitDirection, Vector3 playerDirection);

    public Vector3 Position { get; }

}