using Fusion;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] LobbyScreen LobbyPrefab;

    public void StartMatch(bool isHost)
    {
        Provider.Instance.GameState.StartMultiplayerMatch(isHost ? GameMode.Host : GameMode.Client);
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