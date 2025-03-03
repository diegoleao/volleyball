using Fusion;
using UnityEngine;
using UnityEngine.Events;

public interface IVolleyballGameplay
{
    public bool HasStateAuthority { get; }

    public void InjectMatchInfo(MatchInfo matchInfo);

    public void SpawnVolleyball(Team team);

    public void SpawnVolleyball(GameObject volleyBallPrefab, CourtTriggers courtTriggers, Team team);

    public void ResetPlayerPositions();

    public void ShutdownNetworkMatch();

    public void ResetMatch();


}