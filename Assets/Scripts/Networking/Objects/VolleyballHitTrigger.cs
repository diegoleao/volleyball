using UnityEngine;
using UnityEngine.Events;

public class VolleyballHitTrigger : MonoBehaviour
{
    [SerializeField] MonoBehaviour volleybal;
    public IVolleyball Volleyball => (IVolleyball)this.volleybal;

}