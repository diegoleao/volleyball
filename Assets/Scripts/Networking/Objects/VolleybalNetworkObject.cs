using Fusion;
using UnityEngine;

public class VolleybalNetworkObject : NetworkBehaviour
{

    [SerializeField] BaseVolleyball volleyball;

    public bool IsGrounded => volleyball.IsGrounded;
    public bool IsGroundChecking => volleyball.IsGroundChecking;
    public float SpawnHeight => volleyball.SpawnHeight;

    public override void Spawned()
    {

    }

}