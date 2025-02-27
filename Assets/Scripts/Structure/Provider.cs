
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Provider : MonoBehaviour
{

    [SerializeField] GameState gameState;
    public GameState GameState => this.gameState;

    [SerializeField] BasicSpawner basicSpawner;
    public BasicSpawner BasicSpawner => this.basicSpawner;

    private static Provider instance;
    public static Provider Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Provider>();
            }
            return instance;
            
        }
    }

    void Start()
    {
        this.gameState.Initialize();

    }

}