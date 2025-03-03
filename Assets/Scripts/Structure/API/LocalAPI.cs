using Fusion;
using UnityEngine;
using System.Linq;
using Sirenix.Utilities;
using System;

public class LocalAPI : MonoBehaviour, IVolleyballGameplay
{
    [SerializeField] PlayerLocal _playerPrefab;

    [SerializeField] Volleyball _volleyBallPrefab;

    public bool HasStateAuthority => true;

    private GameState gameState;

    private VolleyJoystick volleyJoystick;

    private MatchInfo matchInfo;

    void Awake()
    {
        this.gameState = Provider.Instance.GameState;
        this.volleyJoystick = Provider.Instance.VolleyJoystick;

    }

    public void StartLocalMultiplayerMatch()
    {
        InstantiatePlayer(Team.A, isAI: false);
        InstantiatePlayer(Team.B, isAI: false);

    }

    public void StartSingleplayerMatch()
    {
        InstantiatePlayer(Team.A, isAI: false);
        InstantiatePlayer(Team.B, isAI: true);

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
        DestroyAllPlayers();
        DestroyAllBalls();
        DestroyMatch();
    }

    public void SpawnBall(NetworkVolleyball volleyBall, CourtTriggers courtTriggers, Team team, float height)
    {

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

    public void InjectMatchInfo(MatchInfo matchInfo)
    {
        this.matchInfo = matchInfo;

    }

    private void DestroyMatch()
    {
        Destroy(this.matchInfo?.gameObject);

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