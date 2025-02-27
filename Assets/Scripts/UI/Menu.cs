using Fusion;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public void StartMatch(bool isHost)
    {
        Provider.Instance.GameState.StartMatch(isHost ? GameMode.Host : GameMode.Client);
        Close();

    }

    public void Close()
    {
        Destroy(gameObject);
    }

}