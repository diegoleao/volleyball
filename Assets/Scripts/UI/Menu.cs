using Fusion;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] LobbyScreen LobbyPrefab;

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
        Instantiate(LobbyPrefab);
        this.Close();
    }

    public void Close()
    {
        Destroy(gameObject);
    }

}