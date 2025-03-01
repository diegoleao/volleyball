using UnityEngine;

public class CourtTriggers : MonoBehaviour
{

    [SerializeField] BoxCollider teamA;
    [SerializeField] BoxCollider teamB;

    public Vector3 GetTeamSpawnPosition(bool isPlayerMyself)
    {
        if (isPlayerMyself)
        {
            return GetRandomPosition(teamA);
        }
        else
        {
            return GetRandomPosition(teamB);
        }
        
    }

    public Vector3 GetRandomPosition(BoxCollider area)
    {
        return new Vector3(
            Random.Range(area.bounds.min.x, area.bounds.max.x),
            Random.Range(area.bounds.min.y, area.bounds.max.y),
            Random.Range(area.bounds.min.z, area.bounds.max.z)
        );
    }

}