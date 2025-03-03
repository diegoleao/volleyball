using Fusion;
using Sirenix.OdinInspector;
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    [SerializeField] float height;

    [Header("Prefabs")]
    [SerializeField] Volleyball volleyBall;

    private CourtTriggers courtTriggers;

    private Vector3 forward;


    void Awake()
    {
        courtTriggers = FindAnyObjectByType<CourtTriggers>();

    }

    [Button]
    public void SpawnVolleyball(Team team)
    {
        Provider.Instance.API.SpawnBall(this.volleyBall, this.courtTriggers, team, height);

    }

}