
using UnityEngine;

public class CourtFloorPointTrigger : MonoBehaviour
{
    [SerializeField] private Team TargetOfWichTeam;

    private BaseVolleyball volleyball;

    public void OnTriggerEnter(Collider other)
    {
        volleyball = other.GetComponent<BaseVolleyball>();
        if (volleyball != null)
        {
            volleyball.HandleGroundTouch(TargetOfWichTeam);
        }

    }

}