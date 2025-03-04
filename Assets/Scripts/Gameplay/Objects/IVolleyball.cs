
public interface IVolleyball
{
    public bool IsGrounded { get; }

    public void HandleGroundTouch(Team scoringTeam);

}