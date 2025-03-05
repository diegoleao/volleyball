
using Fusion;
using UnityEngine;

public class NetworkMovement : MonoBehaviour
{

    private NetworkCharacterController netCharController;
    private NetworkRunner runner;

    public void Initialize(NetworkRunner runner)
    {
        this.runner = runner;
        netCharController = GetComponent<NetworkCharacterController>();

    }

    public void MoveNetworkCharacter(Vector3 direction)
    {
        direction.Normalize();
        netCharController.Move(direction * runner.DeltaTime);

    }

}