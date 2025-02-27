

using UnityEngine;

public class Menu : MonoBehaviour
{
    public void StartMatch()
    {
        Provider.Instance.GameState.SetState(GameState.State.SpawnBall);
    }

    public void Close()
    {
        Destroy(gameObject);
    }

}