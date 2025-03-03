
using UnityEngine;

public class CourtFloor : MonoBehaviour
{
    [SerializeField] private Team TargetOfWichTeam;

    private IVolleyball volleyball;

    public void OnTriggerEnter(Collider other)
    {
        volleyball = other.GetComponent<NetworkVolleyball>();
        if (volleyball != null)
        {
            volleyball.HandleGroundTouch(TargetOfWichTeam);
        }

    }

}