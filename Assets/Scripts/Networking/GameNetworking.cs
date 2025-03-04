using Fusion;
using Fusion.Addons.Physics;
using Fusion.Sockets;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameNetworking : MonoBehaviour, IVolleyballGameplay, INetworkRunnerCallbacks
{

    [SerializeField] int requiredPlayers = 2;

    [SerializeField] NetworkPrefabRef _playerPrefab;

    [SerializeField] GameObject _volleybalPrefab;

    [SerializeField] NetworkPrefabRef _matchInfoPrefab;

    public bool HasStateAuthority
    {
        get
        {
            if (_runner == null) return false;
            return _runner.IsServer;
        }
    }


    //Private
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    private NetworkRunner _runner;

    private NetworkInputData inputData;

    private MatchInfo matchInfo;

    private bool fireButton;

    private bool jumpButton;

    private int playersInGame = 0;

    private GameState gameState;

    private VolleyJoystick volleyJoystick;

    void Awake()
    {
        this.gameState = Provider.Instance.GameState;
        this.volleyJoystick = Provider.Instance.VolleyJoystick;

    }

    public async void StartNetworkGame(string roomName, GameMode gameMode, UnityAction finished = null, UnityAction error = null)
    {
        _runner = CreateNetworkRunner();

        CreateRunnerPhysics();

        SceneRef sceneRef = CreateNetworkedScene();

        await StartOrJoinGameSession(gameMode, sceneRef, sessionName: roomName);

        if (HasStateAuthority)
        {
            _runner.Spawn(_matchInfoPrefab, Vector3.zero, Quaternion.identity);

        }

        Debug.Log($"ProvideInput set to: {_runner.ProvideInput}, IsClient: {_runner.IsClient}");

        if (_runner == null)
        {
            error?.Invoke();
        }

        finished?.Invoke();

    }

    private async Task StartOrJoinGameSession(GameMode mode, SceneRef scene, string sessionName)
    {
        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = sessionName,
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });

    }

    [Button]
    public async void ShutdownNetworkMatch()
    {
        if (_runner != null && _runner.IsServer)
        {
            Debug.Log("Ending match...");

            foreach (var player in _runner.ActivePlayers)
            {
                _runner.Disconnect(player);
            }

        }

        await _runner.Shutdown();
        Debug.Log("Match ended.");

    }

    [Button]
    public void ResetMatch()
    {
        ResetPlayerPositions();
        DestroyAllBalls();
        matchInfo?.ResetNetworkedScore();

    }

    public void DestroyAllBalls()
    {
        if (HasStateAuthority)
        {
            var allVolleyballs = FindObjectsByType<NetworkVolleyball>(sortMode: FindObjectsSortMode.None);
            allVolleyballs.Where(t => !t.IsGrounded).ForEach(t => { _runner.Despawn(t.GetComponent<NetworkObject>()); });
        }

    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        playersInGame++;
        if (runner.IsServer)
        {
            NetworkObject networkPlayerObject 
                = runner.Spawn(_playerPrefab, GetTeamSpawnPosition(player), Quaternion.identity, player);

            networkPlayerObject.transform.rotation = LocalAPI.GetInitialRotation(networkPlayerObject.transform.position);

            // Keep track of the player avatars for easy access
            _spawnedCharacters.Add(player, networkPlayerObject);

            if ((playersInGame >= requiredPlayers) && !matchInfo.HasMatchStarted)
            {
                matchInfo.HasMatchStarted = true;

            }
        }

    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        playersInGame--;
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
        }

    }

    private void Update()
    {
#if UNITY_STANDALONE_WIN
        fireButton = fireButton | Input.GetMouseButtonDown(0) | Input.GetButtonDown("Fire1");
        jumpButton = jumpButton | Input.GetMouseButtonDown(1) | Input.GetButtonDown("Jump");
#else
        fireButton = fireButton | this.volleyJoystick.GetFireButton();
        jumpButton = jumpButton | this.volleyJoystick.GetJumpButton();
#endif

    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (matchInfo == null || matchInfo.IsMatchFinished || !matchInfo.HasMatchStarted)
        {
            //Debug.Log($"MATCH FINISHED - IGNORING Input.");
            return;
        }

#if UNITY_STANDALONE_WIN
        inputData = new NetworkInputData();

        inputData.direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        inputData.buttons.Set(NetworkInputData.BUTTON_0_FIRE, fireButton);
        fireButton = false;

        inputData.buttons.Set(NetworkInputData.BUTTON_1_JUMP, jumpButton);
        jumpButton = false;
#else
        inputData = new NetworkInputData();

        inputData.direction = new Vector3(this.volleyJoystick.Horizontal, 0, this.volleyJoystick.Vertical);

        inputData.buttons.Set(NetworkInputData.BUTTON_0_FIRE, fireButton);
        fireButton = false;

        inputData.buttons.Set(NetworkInputData.BUTTON_1_JUMP, jumpButton);
        jumpButton = false;

#endif

        input.Set(inputData);

    }

    public void SpawnVolleyball(Team team)
    {
        SpawnVolleyball(this._volleybalPrefab, Provider.Instance.CourtTriggers, team);
    }

    public void SpawnVolleyball(GameObject volleyBallPrefab, CourtTriggers courtTriggers, Team team)
    {
        Debug.Log("Spawn Volleyball");

        if (FindObjectsOfType<NetworkVolleyball>().Any(t => !t.IsGroundChecking))
            return;

        if (HasStateAuthority)
        {
            _runner.Spawn(volleyBallPrefab,
                         courtTriggers.GetBallSpawnPosition(team, volleyBallPrefab.GetComponent<NetworkVolleyball>().SpawnHeight),
                         Quaternion.identity,
                         null,
                         (runner, obj) =>
                         {

                         });

        }

    }

    [Button]
    public void ResetPlayerPositions()
    {
        if (HasStateAuthority)
        {
            _spawnedCharacters.ForEach(character =>
            {
                ResetPlayerToInitialPosition(character.Key, character.Value);
            });
        }

    }

    private void ResetPlayerToInitialPosition(PlayerRef player, NetworkObject networkPlayerObject)
    {
        Vector3 initialSpawnPosition = GetTeamSpawnPosition(player);
        networkPlayerObject.GetComponent<NetworkCharacterController>()
                           .Teleport(initialSpawnPosition,
                                     LocalAPI.GetInitialRotation(initialSpawnPosition));

    }

    private Vector3 GetTeamSpawnPosition(PlayerRef player, float spawnHeight = 1)
    {
        //TODO: FIX THIS CALCULATION, THE TEAM A IS NOT ALWAYS THE CURRENT PLAYER
        return LocalAPI.GetTeamSpawnPosition((player == _runner.LocalPlayer) ? Team.A : Team.B);

    }

    private NetworkRunner CreateNetworkRunner()
    {
        var runner = gameObject.AddComponent<NetworkRunner>();

        //Let it know that we will be providing user input
        runner.ProvideInput = true;

        return runner;

    }

    private void CreateRunnerPhysics()
    {
        var runnerSimulatePhysics3D = gameObject.AddComponent<RunnerSimulatePhysics3D>();
        runnerSimulatePhysics3D.ClientPhysicsSimulation = ClientPhysicsSimulation.SimulateAlways;

    }

    private static SceneRef CreateNetworkedScene()
    {
        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        return scene;
    }

    public void InjectMatchInfo(MatchInfo matchInfo)
    {
        this.matchInfo = matchInfo;

    }
    public void InjectMatchInfo(LocalMatchInfo matchInfo)
    {

    }

    void OnApplicationQuit()
    {
        if(_runner != null)
        {
            ShutdownNetworkMatch();
        }

    }
    public void UnloadScene() { }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("Client successfully connected to server.");
    }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }

}