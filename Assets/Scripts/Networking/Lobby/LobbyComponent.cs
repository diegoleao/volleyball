
using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class LobbyComponent : MonoBehaviour, INetworkRunnerCallbacks
{

    public List<SessionInfo> SessionList { get; private set; } = new List<SessionInfo>();

    public UnityEvent<List<SessionInfo>> SessionListUpdatedEvent;

    private NetworkRunner runner;

    public async void Initialize()
    {
        runner = gameObject.AddComponent<NetworkRunner>();
        Provider.Register<LobbyComponent>(this);
        await JoinLobby(runner);

    }

    public async Task JoinLobby(NetworkRunner runner)
    {
        var result = await runner.JoinSessionLobby(SessionLobby.ClientServer);

        if (result.Ok)
        {
            Debug.Log($"Started in Lobby");
        }
        else
        {
            Debug.LogError($"Failed to Start: {result.ShutdownReason}");
        }

    }

    public void JoinSession(string sessionName)
    {
        Provider.Instance.GameState.StartMultiplayerMatch(sessionName, GameMode.Client);

    }

    public bool QuickJoinFirstSession(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        if (sessionList.Count == 0)
        {
            return false;

        }

        var session = sessionList[0];
        
        Debug.Log($"Joining {session.Name}");

        JoinSession(session.Name);

        return true;

    }

    //Interface implementation ==========================================

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log($"Session List Updated with {sessionList.Count} session(s)");
        this.SessionList = sessionList;
        this.SessionListUpdatedEvent?.Invoke(this.SessionList);

    }


    //Not used ===========================================================


    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        
    }

}