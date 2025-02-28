
using UnityEngine;

public class CourtFloor : MonoBehaviour
{
    [SerializeField] private GameState.Team TargetOfWichTeam;
    private PhysxBall physxBall;

    public void OnTriggerEnter(Collider other)
    {
        physxBall = other.GetComponent<PhysxBall>();
        if (physxBall != null)
        {
            Provider.Instance.GameState.IncreaseScoreFor(TargetOfWichTeam);
        }
        physxBall.StopMoving();

    }

}