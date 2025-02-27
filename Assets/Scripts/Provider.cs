
using UnityEngine;

public class Provider : MonoBehaviour
{
    [SerializeField] MatchInfo gameStateInfo;

    [SerializeField] GameState gameState;
    public GameState GameState => this.gameState;

    [SerializeField] BallSpawner ballSpawner;
    public BallSpawner BallSpawner => this.ballSpawner;

    public static Provider Instance
    {
        get
        {
            return FindObjectOfType<Provider>();
        }
    }

    private void Start()
    {
        this.gameState.SetState(GameState.State.Menu);

    }

}