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
    private bool isTouchingVolleyball;
    private VolleyballHitTrigger possibleBallTrigger;


    void Awake()
    {
        netCharController = GetComponent<NetworkCharacterController>();
        jumpComponent = GetComponent<JumpComponent>();
        forward = Vector3.forward;

    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            MoveCharacter(data);
            HandleButtonPress(data);
        }

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
            
            if (data.buttons.IsSet(NetworkInputData.BUTTON_0_FIRE) && delay.ExpiredOrNotRunning(Runner))
            {
                Debug.Log("BUTTON_0_FIRE Pressed");
                delay = TickTimer.CreateFromSeconds(Runner, ballHittingDelay);

                this.transform.position = transform.position + (forward.normalized * 0.05f);

                if(isTouchingVolleyball && volleyball != null)
                {
                    Debug.Log($"Applyed impulse to {volleyball.name}");
                    volleyball.ApplyImpulse(this.transform.forward, this.transform.forward);
                }

            }

            if (isTouchingVolleyball)
            {
                isTouchingVolleyball = false;
            }

        }

    }

    private void MoveCharacter(NetworkInputData data)
    {
        data.direction.Normalize();

        netCharController.Move(data.direction * Runner.DeltaTime);

    }

    public void OnTriggerEnter(Collider other)
    {
        possibleBallTrigger = other.GetComponent<VolleyballHitTrigger>();
        if (possibleBallTrigger != null)
        {
            if (!possibleBallTrigger.Volleyball.IsGrounded)
            {
                isTouchingVolleyball = true;
                volleyball = possibleBallTrigger.Volleyball;
                Debug.Log($"Hit {other.name}");
            }
            else
            {
                isTouchingVolleyball = false;
            }

        }

    }

    //    public void SetVolleyballColliding(Volleyball volleyball, bool isTriggerColliding)
    //    {
    //        if (isTriggerColliding)
    //        {
    //            //Debug.Log($"Volleyball {volleyball.name} touched player.");
    //s            this.volleyball = volleyball;
    //        }
    //        else
    //        {
    //            Debug.Log($"Volleyball {volleyball.name} not touching player anymore.");
    //            this.volleyball = null;
    //        }

    //    }

}