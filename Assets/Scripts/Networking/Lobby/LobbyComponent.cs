
using Fusion;
using System.Threading.Tasks;
using UnityEngine;

public class LobbyComponent : MonoBehaviour
{

    NetworkRunner runner;

    async void Start()
    {
        runner = CreateNetworkRunner();
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

    private NetworkRunner CreateNetworkRunner()
    {
        return gameObject.AddComponent<NetworkRunner>();

    }

}