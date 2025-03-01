
using UnityEngine;

public class CourtFloor : MonoBehaviour
{
    [SerializeField] private GameState.Team TargetOfWichTeam;
    private Volleyball physxBall;

    public void OnTriggerEnter(Collider other)
    {
        physxBall = other.GetComponent<Volleyball>();
        if (physxBall != null)
        {
            Provider.Instance.GameState.IncreaseScoreFor(TargetOfWichTeam);
            physxBall.StopMoving();
        }

    }

}