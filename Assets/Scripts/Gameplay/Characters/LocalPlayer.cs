using Fusion;
using Sirenix.OdinInspector;
using System;
using UniRx;
using UnityEngine;

[RequireComponent(typeof(JumpComponent), typeof(JoystickSingleplayer), typeof(PlayerMovement))]
public class LocalPlayer : MonoBehaviour, IPlayer
{
    public Team Team { get; private set; }

    private bool isInitialized;
    private bool isTouchingVolleyball;
    private LocalVolleyball volleyball;
    private VolleyballHitTrigger possibleBallTrigger;
    private JoystickSingleplayer joystickSingleplayer;
    private BallHitting ballHitting;

    public void Initialize(Team team)
    {
        joystickSingleplayer = GetComponent<JoystickSingleplayer>();
        joystickSingleplayer.Initialize(team);

        ballHitting = GetComponent<BallHitting>();
        ballHitting.Initialize(team);

        GetComponent<PlayerMovement>().Initialize();

        GetComponent<JumpComponent>().Initialize();

        this.Team = team;

        isInitialized = true;

    }

    public void InjectVolleyball(IVolleyball volleyball)
    {
        this.volleyball = (LocalVolleyball)volleyball;
        ballHitting.InjectVolleyball(this.volleyball);

    }

    void Update()
    {
        if (!isInitialized)
            return;

        if (joystickSingleplayer.GetFireButtonDown())
        {
            Debug.Log("[Ball-Player] BUTTON_0_FIRE Pressed");
            ballHitting.HitTheBall();

        }

    }

}