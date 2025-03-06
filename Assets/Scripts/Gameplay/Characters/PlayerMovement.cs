using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : BaseMovement
{
    private JoystickSingleplayer joystickSingleplayer;

    public override void Initialize(Team team)
    {
        joystickSingleplayer = GetComponent<JoystickSingleplayer>();
        base.Initialize(team);

    }

    protected override void UpdateMovement()
    {
        if (!isInitialized)
            return;

        moveDirection = new Vector3(joystickSingleplayer.Horizontal, 0, joystickSingleplayer.Vertical);

    }

}
