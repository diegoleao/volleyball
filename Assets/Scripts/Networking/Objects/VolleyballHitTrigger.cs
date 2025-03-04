using UnityEngine;
using UnityEngine.Events;

public class VolleyballHitTrigger : MonoBehaviour
{
    [SerializeField] NetworkVolleyball volleybal;
    public NetworkVolleyball Volleyball => this.volleybal;

    [SerializeField] LocalVolleyball localVolleybal;
    public LocalVolleyball LocalVolleybal => this.localVolleybal;

}