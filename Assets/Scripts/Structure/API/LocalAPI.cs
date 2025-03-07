using Fusion;
using UnityEngine;
using System.Linq;
using Sirenix.Utilities;
using System;

public class LocalAPI : MonoBehaviour, IVolleyballGameplay
{
    [SerializeField] LocalPlayer _playerPrefab;

    [SerializeField] AIPlayer _AIPlayer;

    [SerializeField] LocalVolleyball _volleyBallPrefab;

    [SerializeField] LocalMatchInfo _localMatchInfoPrefab;

    public bool HasStateAuthority => true;

    private GameState gameState;

    private VirtualJoystick volleyJoystick;

    private LocalMatchInfo localMatchInfo;

    void Awake()
    {
        this.gameState = Provider.GameState;
        this.volleyJoystick = Provider.VolleyJoystick;
    }

    public void StartLocalMultiplayerMatch()
    {
        InstantiatePlayer(Team.A);
        InstantiatePlayer(Team.B);
        Provider.GameState.StartGameplay();
        this.localMatchInfo = Instantiate(_localMatchInfoPrefab, this.transform);
        this.localMatchInfo.InitializeLocal();
        Provider.Register<LocalMatchInfo>(this.localMatchInfo);
    }

    public void StartSingleplayerMatch()
    {
        InstantiatePlayer(Team.A);
        InstantiateAI(Team.B);
        Provider.GameState.StartGameplay();
        this.localMatchInfo = Instantiate(_localMatchInfoPrefab, this.transform);
        this.localMatchInfo.InitializeLocal();
        Provider.Register<LocalMatchInfo>(this.localMatchInfo);

    }

    public void ResetMatch()
    {
        ResetPlayerPositions();
        DestroyAllBalls();
        this.localMatchInfo.ResetScore();

    }

    public void ResetPlayerPositions()
    {
        FindObjectsOfType<LocalPlayer>().ToList().ForEach(character =>
        {
            ResetPlayerToInitialPosition(character.transform, character.Team);
        });
        FindObjectsOfType<AIPlayer>().ToList().ForEach(character =>
        {
            ResetPlayerToInitialPosition(character.transform, character.Team);
        });

    }

    public void ShutdownNetworkMatch()
    {
        UnloadScene();
    }

    public void UnloadScene()
    {
        DestroyAllPlayers();
        DestroyAllNonGroundedBalls();
        DestroyMatch();

    }

    public void SpawnVolleyball(Team team)
    {
        this.SpawnVolleyball(_volleyBallPrefab.gameObject, Provider.CourtTriggers, team);

    }

    public void SpawnVolleyball(GameObject volleyBallPrefab, CourtTriggers courtTriggers, Team team)
    {
        Debug.Log("Spawn Volleyball");

        if (FindObjectsOfType<LocalVolleyball>().Any(t => !t.IsGroundChecking))
            return;

        Instantiate(volleyBallPrefab,
            Provider.CourtTriggers.GetBallSpawnPosition(team, _volleyBallPrefab.SpawnHeight), 
            Quaternion.identity);

    }

    private void ResetPlayerToInitialPosition(Transform player, Team team)
    {
        player.position = GetTeamSpawnPosition(team);
        player.rotation = GetInitialRotation(player.transform.position);

    }

    public static Vector3 GetTeamSpawnPosition(Team team, float spawnHeight = 1)
    {
        return Provider.CourtTriggers
                       .GetTeamSpawnPosition(team, spawnHeight);

    }

    public static Quaternion GetInitialRotation(Vector3 currentPosition)
    {
        Vector3 targetPositionToLookAt = Provider.CourtCenter.position;
        targetPositionToLookAt.y = currentPosition.y; // Prevent character from bending up or down

        return Quaternion.LookRotation(targetPositionToLookAt - currentPosition, Vector3.up);

    }

    private void DestroyAllPlayers()
    {
        DestroyAll<LocalPlayer>();
        DestroyAll<AIPlayer>();

    }

    public void DestroyAllNonGroundedBalls()
    {
        var allVolleyballs = FindObjectsOfType<LocalVolleyball>();
        allVolleyballs.Where(t => !t.IsGrounded).ForEach(nonGrounded => { Destroy(nonGrounded.gameObject); });

    }

    public void DestroyAllBalls()
    {
        var allVolleyballs = FindObjectsOfType<LocalVolleyball>();
        allVolleyballs.ForEach(ball => { Destroy(ball.gameObject); });

    }

    public void InjectMatchInfo(LocalMatchInfo localMatchInfo)
    {
        this.localMatchInfo = localMatchInfo;

    }

    public void InjectMatchInfo(MatchInfo matchInfo) { }

    private void DestroyMatch()
    {
        if(this.localMatchInfo != null)
            Destroy(this.localMatchInfo.gameObject);

    }

    private void InstantiatePlayer(Team team)
    {
        var player = Instantiate(_playerPrefab,
                                 GetTeamSpawnPosition(team),
                                 Quaternion.identity);

        player.Initialize(team);

        player.transform.rotation = LocalAPI.GetInitialRotation(player.transform.position);

    }

    private void InstantiateAI(Team team)
    {
        var player = Instantiate(_AIPlayer,
                                 GetTeamSpawnPosition(team),
                                 Quaternion.identity);

        player.Initialize(team);

        player.transform.rotation = LocalAPI.GetInitialRotation(player.transform.position);

    }

    private static void DestroyAll<T>() where T : MonoBehaviour
    {
        var all = FindObjectsOfType<T>();
        all.ForEach(t => { Destroy(t.gameObject); });

    }

}