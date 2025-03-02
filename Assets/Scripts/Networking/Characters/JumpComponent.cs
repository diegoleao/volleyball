using Fusion;
using UnityEngine;

public class JumpComponent : NetworkBehaviour
{
    [SerializeField] private float jumpForce = 10f;
    private NetworkCharacterController netCharController;

    void Awake()
    {
        netCharController = GetComponent<NetworkCharacterController>();

    }

    public void Jump()
    {
        if (netCharController.Grounded)
        {
            Vector3 velocity = netCharController.Velocity;
            velocity.y = jumpForce;
            netCharController.Velocity = velocity;
            Debug.Log("Player jumped!");

        }

    }

}