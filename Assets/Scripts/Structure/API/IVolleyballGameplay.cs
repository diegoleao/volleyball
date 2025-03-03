using Fusion;
using UnityEngine.Events;

public interface IVolleyballGameplay
{
    public bool HasStateAuthority { get; }

    public void InjectMatchInfo(MatchInfo matchInfo);

    public void SpawnBall(Volleyball volleyBall, CourtTriggers courtTriggers, Team team, float height);

    public void ResetPlayerPositions();

    public void ShutdownNetworkMatch();

    public void ResetMatch();


}