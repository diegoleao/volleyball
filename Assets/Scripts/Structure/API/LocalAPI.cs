using Fusion;
using UnityEngine;
using System.Linq;
using Sirenix.Utilities;
using System;

public class LocalAPI : MonoBehaviour, IVolleyballGameplay
{
    [SerializeField] PlayerLocal _playerPrefab;

    [SerializeField] Volleyball _volleyBallPrefab;

    [SerializeField] LocalMatchInfo _localMatchInfoPrefab;

    public bool HasStateAuthority => true;

    private GameState gameState;

    private VolleyJoystick volleyJoystick;

    private LocalMatchInfo localMatchInfo;

    void Awake()
    {
        this.gameState = Provider.Instance.GameState;
        this.volleyJoystick = Provider.Instance.VolleyJoystick;
    }

    public void StartLocalMultiplayerMatch()
    {
        InstantiatePlayer(Team.A, isAI: false);
        InstantiatePlayer(Team.B, isAI: false);
        Provider.Instance.GameState.StartGameplay();
        this.localMatchInfo = Instantiate(_localMatchInfoPrefab, this.transform);
        this.localMatchInfo.InitializeLocal();
        Provider.Register<LocalMatchInfo>(this.localMatchInfo);
    }

    public void StartSingleplayerMatch()
    {
        InstantiatePlayer(Team.A, isAI: false);
        InstantiatePlayer(Team.B, isAI: true);
        Provider.Instance.GameState.StartGameplay();
        this.localMatchInfo = Instantiate(_localMatchInfoPrefab, this.transform);
        this.localMatchInfo.InitializeLocal();
        Provider.Register<LocalMatchInfo>(this.localMatchInfo);

    }

    public void ResetMatch()
    {
        ResetPlayerPositions();
        DestroyAllBalls();

    }

    public void ResetPlayerPositions()
    {
        FindObjectsOfType<Player>().ToList().ForEach(character =>
        {
            ResetPlayerToInitialPosition(character);
        });

    }

    public void ShutdownNetworkMatch()
    {
        UnloadScene();
    }

    public void UnloadScene()
    {
        DestroyAllPlayers();
        DestroyAllBalls();
        DestroyMatch();

    }

    public void SpawnVolleyball(Team team)
    {
        this.SpawnVolleyball(_volleyBallPrefab.gameObject, Provider.Instance.CourtTriggers, team);

    }

    public void SpawnVolleyball(GameObject volleyBallPrefab, CourtTriggers courtTriggers, Team team)
    {
        Debug.Log("Spawn Volleyball");

        if (FindObjectsOfType<Volleyball>().Any(t => !t.IsGroundChecking))
            return;

        Instantiate(volleyBallPrefab,
            Provider.Instance.CourtTriggers.GetBallSpawnPosition(team, _volleyBallPrefab.SpawnHeight), 
            Quaternion.identity);

    }

    private void ResetPlayerToInitialPosition(Player player)
    {
        player.transform.position = GetTeamSpawnPosition(player.Team);
        player.transform.rotation = GetInitialRotation(player.transform.position);

    }

    public static Vector3 GetTeamSpawnPosition(Team team, float spawnHeight = 1)
    {
        return Provider.Instance
                       .CourtTriggers
                       .GetTeamSpawnPosition(team, spawnHeight);

    }

    public static Quaternion GetInitialRotation(Vector3 currentPosition)
    {
        Vector3 targetPositionToLookAt = Provider.Instance.CourtCenter.position;
        targetPositionToLookAt.y = currentPosition.y; // Prevent character from bending up or down

        return Quaternion.LookRotation(targetPositionToLookAt - currentPosition, Vector3.up);

    }

    private void DestroyAllPlayers()
    {
        var allVolleyballs = FindObjectsOfType<Player>();
        allVolleyballs.ForEach(player => { Destroy(player.gameObject); });

    }

    public void DestroyAllBalls()
    {
        var allVolleyballs = FindObjectsOfType<NetworkVolleyball>();
        allVolleyballs.Where(t => !t.IsGrounded).ForEach(nonGrounded => { Destroy(nonGrounded.gameObject); });

    }

    public void InjectMatchInfo(LocalMatchInfo localMatchInfo)
    {
        this.localMatchInfo = localMatchInfo;

    }

    public void InjectMatchInfo(MatchInfo matchInfo) { }

    private void DestroyMatch()
    {
        Destroy(this.localMatchInfo?.gameObject);

    }

    private void InstantiatePlayer(Team team, bool isAI)
    {
        var player = Instantiate(_playerPrefab,
                                 GetTeamSpawnPosition(team), 
                                 Quaternion.identity);

        player.Initialize(team, isAI);

        player.transform.rotation = LocalAPI.GetInitialRotation(player.transform.position);

    }

}