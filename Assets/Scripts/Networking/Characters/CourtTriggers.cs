using UnityEngine;

public class CourtTriggers : MonoBehaviour
{
    [Header("Team A")]
    [SerializeField] BoxCollider teamAPlayerSpawn;
    [SerializeField] BoxCollider teamABallVolume;

    [Header("Team B")]
    [SerializeField] BoxCollider teamBPlayerSpawn;
    [SerializeField] BoxCollider teamBBallVolume;

    public Vector3 GetTeamSpawnPosition(Team team, float height)
    {
        return GetRandomPositionInBox(teamAPlayerSpawn, teamBPlayerSpawn, height, team);

    }

    public Vector3 GetBallSpawnPosition(Team team, float height)
    {
        return GetRandomPositionInBox(teamABallVolume, teamBBallVolume, height, team);

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