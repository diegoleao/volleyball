using Fusion;
using Fusion.Addons.Physics;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameNetworking : MonoBehaviour, INetworkRunnerCallbacks
{

    [SerializeField] NetworkPrefabRef _playerPrefab;

    [SerializeField] NetworkPrefabRef MatchInfoPrefab;

    public MatchInfo MatchInfo { get; private set; }

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

    private void CreateMatchInformationComponent()
    {
        if (HasStateAuthority)
        {
            this.MatchInfo = _runner.Spawn(MatchInfoPrefab, Vector3.zero, Quaternion.identity).GetComponent<MatchInfo>();

        }

    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            // Create a unique position for the player
            Vector3 spawnPosition = Provider.Instance.CharacterSpawner.GetTeamSpawnPos(player == runner.LocalPlayer);

            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);

            Vector3 targetRotation = Provider.Instance.CourtCenter.position;
            targetRotation.y = networkPlayerObject.transform.position.y;
            networkPlayerObject.transform.LookAt(targetRotation, Vector3.up);

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

    private async System.Threading.Tasks.Task StartOrJoinGameSession(GameMode mode, SceneRef scene, string sessionName)
    {
        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = sessionName,
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
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
        var data = new NetworkInputData();

        if (Input.GetKey(KeyCode.W))
            data.direction += Vector3.forward;

        if (Input.GetKey(KeyCode.S))
            data.direction += Vector3.back;

        if (Input.GetKey(KeyCode.A))
            data.direction += Vector3.left;

        if (Input.GetKey(KeyCode.D))
            data.direction += Vector3.right;

        data.buttons.Set(NetworkInputData.MOUSEBUTTON0, _mouseButton0);
        _mouseButton0 = false;

        data.buttons.Set(NetworkInputData.MOUSEBUTTON1, _mouseButton1);
        _mouseButton1 = false;

        input.Set(data);

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