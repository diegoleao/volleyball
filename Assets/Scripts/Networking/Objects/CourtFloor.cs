
using UnityEngine;

public class CourtFloor : MonoBehaviour
{
    [SerializeField] private Team TargetOfWichTeam;
    private Volleyball volleyball;

    public void OnTriggerEnter(Collider other)
    {
        volleyball = other.GetComponent<Volleyball>();
        if (volleyball != null)
        {
            volleyball.HandleGroundTouch(TargetOfWichTeam);
        }

    }

}