using UnityEngine;
using UnityEngine.Events;

public class VolleyballHitTrigger : MonoBehaviour
{
    [SerializeField] MonoBehaviour volleybal;
    public BaseVolleyball Volleyball => (BaseVolleyball)this.volleybal;

}