using UnityEngine;

public class CourtTriggers : MonoBehaviour
{

    [SerializeField] BoxCollider teamA;
    [SerializeField] BoxCollider teamB;
    [SerializeField] BoxCollider teamACourtSide;
    [SerializeField] BoxCollider teamBCourtSide;

    public Vector3 GetTeamSpawnPosition(Team team, float height)
    {
        return GetRandomPositionInBox(teamA, teamB, height, team);

    }

    public Vector3 GetBallSpawnPosition(Team team, float height)
    {
        return GetRandomPositionInBox(teamACourtSide, teamBCourtSide, height, team);

    }

    private Vector3 GetRandomPositionInBox(BoxCollider boxA, BoxCollider boxB, float height, Team team)
    {
        if (team == Team.A)
        {
            return GetRandomPosition(boxA, height);
        }
        else
        {
            return GetRandomPosition(boxB, height);
        }

    }

    public Vector3 GetRandomPosition(BoxCollider area, float height)
    {
        return new Vector3(
            Random.Range(area.bounds.min.x, area.bounds.max.x),
            height,
            Random.Range(area.bounds.min.z, area.bounds.max.z)
        );

    }

}