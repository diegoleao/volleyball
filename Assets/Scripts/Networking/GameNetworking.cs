using Fusion;
using Fusion.Addons.Physics;
using Fusion.Sockets;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameNetworking : MonoBehaviour, INetworkRunnerCallbacks
{

    [SerializeField] NetworkPrefabRef _playerPrefab;

    [SerializeField] NetworkPrefabRef MatchInfoPrefab;

    public bool HasStateAuthority
    {
        get
        {
            return _runner.IsServer;
        }
    }

    //Private
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    private NetworkRunner _runner;

    private NetworkInputData auxInput;

    private MatchInfo matchInfo;

    private bool _mouseButton0;

    private bool _mouseButton1;

    public async void StartNetwork(string roomName, GameMode gameMode, UnityAction finished = null)
    {
        _runner = CreateNetworkRunner();

        CreateRunnerPhysics();

        SceneRef sceneRef = CreateNetworkedScene();

        await StartOrJoinGameSession(gameMode, sceneRef, sessionName: roomName);

        CreateMatchInformationComponent();

        finished.Invoke();

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
        if (_runner.IsServer)
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
        matchInfo?.ResetScore();

    }

    private void CreateMatchInformationComponent()
    {
        if (HasStateAuthority)
        {
            _runner.Spawn(MatchInfoPrefab, Vector3.zero, Quaternion.identity).GetComponent<MatchInfo>();
            
        }

    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {

            NetworkObject networkPlayerObject 
                = runner.Spawn(_playerPrefab, GetTeamSpawnPosition(player), Quaternion.identity, player);

            networkPlayerObject.transform.rotation = GetInitialRotation(networkPlayerObject.transform.position);

            // Keep track of the player avatars for easy access
            _spawnedCharacters.Add(player, networkPlayerObject);

        }

    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
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
                                     GetInitialRotation(initialSpawnPosition));

    }

    private Vector3 GetTeamSpawnPosition(PlayerRef player)
    {
        //TODO: FIX THIS CALCULATION, THE TEAM A IS NOT ALWAYS THE CURRENT PLAYER
        return Provider.Instance
                       .CharacterSpawner
                       .GetTeamSpawnPosition(player == _runner.LocalPlayer);

    }

    public Quaternion GetInitialRotation(Vector3 currentPosition)
    {
        Vector3 targetPositionToLookAt = Provider.Instance.CourtCenter.position;
        targetPositionToLookAt.y = currentPosition.y; // Prevent character from bending up or down

        return Quaternion.LookRotation(targetPositionToLookAt - currentPosition, Vector3.up);

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

    private void Update()
    {
        _mouseButton0 = _mouseButton0 | Input.GetMouseButton(0);
        _mouseButton1 = _mouseButton1 | Input.GetMouseButton(1);

    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (matchInfo == null || matchInfo.IsMatchFinished)
        {
            //Debug.Log($"MATCH FINISHED - IGNORING Input.");
            return;
        }

        var data = new NetworkInputData();

        data.direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        data.buttons.Set(NetworkInputData.MOUSEBUTTON0, _mouseButton0);
        _mouseButton0 = false;

        data.buttons.Set(NetworkInputData.MOUSEBUTTON1, _mouseButton1);
        _mouseButton1 = false;

        input.Set(data);

    }

    public void InjectMatchInfo(MatchInfo matchInfo)
    {
        this.matchInfo = matchInfo;

    }

    void OnApplicationQuit()
    {
        if(_runner != null)
        {
            ShutdownNetworkMatch();
        }

    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
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