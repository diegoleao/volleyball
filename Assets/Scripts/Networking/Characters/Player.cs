using Fusion;
using System;
using UnityEngine;

[RequireComponent(typeof(JumpComponent))]
public class Player : NetworkBehaviour
{
    //Inspector
    [Header("Player Attributes")]
    [SerializeField] float speed = 6;
    [SerializeField] float ballHittingDelay = 0.1f;

    [Networked] private TickTimer delay { get; set; }

    //Private
    private NetworkCharacterController netCharController;
    private Vector3 forward;
    private JumpComponent jumpComponent;
    private Volleyball volleyball;

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
            else if (data.buttons.IsSet(NetworkInputData.BUTTON_0_FIRE) && delay.ExpiredOrNotRunning(Runner))
            {
                Debug.Log("Hit volleyball - Player");
                delay = TickTimer.CreateFromSeconds(Runner, ballHittingDelay);

                this.transform.position = transform.position + (forward.normalized * 0.05f);

                if(volleyball != null)
                {
                    volleyball.ApplyImpulse(this.transform.forward, this.transform.forward);
                }
                
            }

        }

    }

    public void InjectVolleyball(Volleyball volleyball)
    {
        this.volleyball = volleyball;

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