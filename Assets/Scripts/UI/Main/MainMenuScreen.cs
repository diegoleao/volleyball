using Fusion;
using UnityEngine;

public class MainMenuScreen : BaseView
{
    [SerializeField] LobbyScreen LobbyPrefab;

    private LobbyScreen lobbyInstance;

    private bool isClosed;


    private void Start()
    {
        CreateHiddenLobby();

    }

    public void StartMatch(bool isHost)
    {
        StartMatch("TestRoom", isHost);

    }

    public void StartMatch(string roomName, bool isHost)
    {
        Provider.Instance.GameState.StartMultiplayerMatch(roomName, isHost ? GameMode.Host : GameMode.Client);
        Close();
    }

    public void StartSinglePlayer()
    {
        Provider.Instance.GameState.StartSingleplayerMatch();
        Close();

    }

    public void EnterLobby()
    {
        this.lobbyInstance.Show();
        this.Hide();
    }

    private void CreateHiddenLobby()
    {
        if (this.lobbyInstance == null)
        {
            this.lobbyInstance = Instantiate(LobbyPrefab);
            this.lobbyInstance.Initialize(this);
        }

    }

    public void Close()
    {
        if (isClosed)
            return;

        isClosed = true;

        this.lobbyInstance.Close();
        Destroy(gameObject);

    }

}