using Fusion;
using Sirenix.OdinInspector;
using System;
using UniRx;
using UnityEngine;

[RequireComponent(typeof(NetworkJumpComponent), typeof(NetworkBallHitting))]
[RequireComponent(typeof(NetworkCharacterController), typeof(NetworkMovement))]
public class NetworkPlayer : NetworkBehaviour
{
    [ShowInInspector][Sirenix.OdinInspector.ReadOnly]
    public Team Team { get; private set; }

    private NetworkVolleyball volleyball;
    private NetworkJumpComponent jumpComponent;
    private NetworkBallHitting networkBallImpulse;
    private NetworkMovement networkMovement;


    public override void Spawned()
    {
        Initialize();

    }

    public void Initialize()
    {
        this.Team = Provider.GameplayFacade.MyNetworkTeam;

        jumpComponent = GetComponent<NetworkJumpComponent>();

        networkBallImpulse = GetComponent<NetworkBallHitting>();
        networkBallImpulse.Initialize(this.Team);

        networkMovement = GetComponent<NetworkMovement>();
        networkMovement.Initialize(Runner);

    }

    public void InjectVolleyball(BaseVolleyball volleyball)
    {
        this.volleyball = (NetworkVolleyball)volleyball;
        this.networkBallImpulse.InjectVolleyball(volleyball);

    }


    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            networkMovement.MoveNetworkCharacter(data.direction);
            HandleNetworkButtonPress(data);
        }

    }

    private void HandleNetworkButtonPress(NetworkInputData data)
    {
        if (HasStateAuthority)
        {
            if (IsNetworkJump(data))
            {
                Debug.Log("[Player] BUTTON_1_JUMP Pressed");
                jumpComponent.Jump();

            }

            if (IsNetworkFireBtn(ref data))
            {
                Debug.Log("[Ball-Player] BUTTON_0_FIRE Pressed");
                networkBallImpulse.HitIt();

            }

        }

    }

    private static bool IsNetworkJump(NetworkInputData data)
    {
        return data.buttons.IsSet(NetworkInputData.BUTTON_1_JUMP);

    }

    private static bool IsNetworkFireBtn(ref NetworkInputData data)
    {
        return data.buttons.IsSet(NetworkInputData.BUTTON_0_FIRE);

    }

}