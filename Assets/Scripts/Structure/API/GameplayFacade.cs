using Fusion;
using UnityEngine;
using UnityEngine.Events;

public class GameplayFacade : MonoBehaviour
{
    public IVolleyballGameplay CurrentAPI { get; private set; }

    public PlayMode PlayMode { get; private set; }

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
        SetMode(PlayMode.Network);
        GameNetworking.StartNetworkGame(roomName, gameMode, finished, error);

    }

    public void StartLocalMatch()
    {
        SetMode(PlayMode.Local);
        this.localAPI.StartLocalMultiplayerMatch();

    }
    public void StartSingleplayerMatch()
    {
        SetMode(PlayMode.Local);
        this.localAPI.StartSingleplayerMatch();

    }

    private void SetMode(PlayMode playMode)
    {
        this.PlayMode = playMode;

        if (playMode == PlayMode.Local)
            CurrentAPI = localAPI;

        if (playMode == PlayMode.Network)
            CurrentAPI = GameNetworking;
        
    }

}

public enum PlayMode
{
    None,
    Local,
    Network
}