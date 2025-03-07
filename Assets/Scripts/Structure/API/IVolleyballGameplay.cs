using Fusion;
using UnityEngine;
using UnityEngine.Events;

public interface IVolleyballGameplay
{

    public void InjectMatchInfo(MatchInfo matchInfo);

    public void InjectMatchInfo(LocalMatchInfo matchInfo);

    public void SpawnVolleyball(Team team);

    public void SpawnVolleyball(GameObject volleyBallPrefab, CourtTriggers courtTriggers, Team team);

    public void ResetPlayerPositions();

    public void ShutdownMatch();

    public void ResetMatch();

    public void UnloadScene();


}