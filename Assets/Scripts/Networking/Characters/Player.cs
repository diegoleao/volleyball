using Fusion;
using UnityEngine;

[RequireComponent(typeof(JumpComponent))]
public class Player : NetworkBehaviour
{
    //Inspector
    [Header("Player Attributes")]
    [SerializeField] float speed = 6;

    //Private
    private NetworkCharacterController netCharController;
    private Vector3 forward;
    private JumpComponent jumpComponent;

    void Awake()
    {
        netCharController = GetComponent<NetworkCharacterController>();
        jumpComponent = GetComponent<JumpComponent>();
        forward = Vector3.forward;

    }

    private void HandleButtonPress(NetworkInputData data)
    {
        if (data.direction.sqrMagnitude > 0)
            forward = data.direction;

        if (HasStateAuthority)
        {
            if (data.buttons.IsSet(NetworkInputData.BUTTON_1_JUMP))
            {
                Debug.Log("Jump pressed - Player");
                jumpComponent.Jump();

            }

        }

    }


    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            MoveCharacter(data);
            HandleButtonPress(data);
        }

    }

    private void MoveCharacter(NetworkInputData data)
    {
        data.direction.Normalize();

        netCharController.Move(data.direction * Runner.DeltaTime);

    }

}