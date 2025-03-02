using UnityEngine;
using UnityEngine.Events;

public class VolleyballHitTrigger : MonoBehaviour
{
    [SerializeField] Volleyball volleybal;
    public Volleyball Volleyball => this.volleybal;

    //[SerializeField] UnityEvent<Collider> collisionDetected;
    //[SerializeField] UnityEvent<Collider> collisionLost;

    //void OnTriggerEnter(Collider other)
    //{
    //    collisionDetected.Invoke(other);

    //}

    //void OnTriggerExit(Collider other)
    //{
    //    collisionLost.Invoke(other);

    //}

}