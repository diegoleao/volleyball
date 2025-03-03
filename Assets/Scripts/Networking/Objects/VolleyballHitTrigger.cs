using UnityEngine;
using UnityEngine.Events;

public class VolleyballHitTrigger : MonoBehaviour
{
    [SerializeField] NetworkVolleyball volleybal;
    public NetworkVolleyball Volleyball => this.volleybal;

    [SerializeField] Volleyball localVolleybal;
    public Volleyball LocalVolleybal => this.localVolleybal;

}