using Fusion;
using System;
using UnityEngine;
using UnityEngine.Events;

public class GameplayFacade : MonoBehaviour
{
    public IVolleyballGameplay CurrentAPI { get; private set; }

    public NetworkMode PlayMode { get; private set; }

    public Team MyNetworkTeam
    {
        get
        {
            return GameNetworking.IsHost ? Team.A : Team.B;
        }
    }

    [Header("Structural")]

    [SerializeField] LocalAPI localAPI;

    private GameNetworking _gameNetworkingInstance;
    [SerializeField] GameNetworking GameNetworkingPrefab;
    public GameNetworking GameNetworking
    {
        get
        {
            if (_gameNetworkingInstance == null)
            {
                _gameNetworkingInstance = Instantiate(GameNetworkingPrefab, this.transform);
            }
            return _gameNetworkingInstance;
        }
    }

    public void StartNetworkMatch(string roomName, GameMode gameMode, UnityAction finished = null, UnityAction error = null)
    {
        SetMode(NetworkMode.Network);
        GameNetworking.StartNetworkGame(roomName, gameMode, finished, error);

    }

    public void StartLocalMatch()
    {
        SetMode(NetworkMode.Local);
        this.localAPI.StartLocalMultiplayerMatch();

    }
    public void StartSingleplayerMatch()
    {
        SetMode(NetworkMode.Local);
        this.localAPI.StartSingleplayerMatch();

    }

    private void SetMode(NetworkMode playMode)
    {
        this.PlayMode = playMode;

        if (playMode == NetworkMode.Local)
            CurrentAPI = localAPI;

        if (playMode == NetworkMode.Network)
            CurrentAPI = GameNetworking;
        
    }

}

public enum NetworkMode
{
    None,
    Local,
    Network
}