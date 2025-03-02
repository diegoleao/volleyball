using Fusion;
using UnityEngine;

public class MainMenuScreen : BaseView
{
    [SerializeField] LobbyScreen LobbyPrefab;

    private LobbyScreen lobbyInstance;

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

    public override void Close()
    {
        if (isClosed)
            return;

        this.lobbyInstance.Close();
        base.Close();

    }

}