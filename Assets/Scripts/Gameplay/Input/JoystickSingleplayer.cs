
using Sirenix.OdinInspector;
using UnityEngine;

public class JoystickSingleplayer : MonoBehaviour
{
    [ShowInInspector][ReadOnly]
    private Team team = Team.None;

    private VirtualJoystick joystick;

    public void Initialize(Team team)
    {
        this.team = team;
        joystick = Provider.VolleyJoystick;

    }

    public float Horizontal
    {
        get
        {
            if (team == Team.A)
            {
#if UNITY_STANDALONE_WIN
                return Input.GetAxisRaw("Horizontal");
#else
                return joystick.Horizontal;
#endif
            }
            else
            {
                return Input.GetAxisRaw("Horizontal_P2");
            }

        }

    }

    public float Vertical
    {
        get
        {
            if (team == Team.A)
            {
#if UNITY_STANDALONE_WIN
                return Input.GetAxisRaw("Vertical");
#else
                return joystick.Vertical;
#endif
            }
            else
            {
                return Input.GetAxisRaw("Vertical_P2");
            }
        }

    }

    public bool GetFireButtonDown()
    {
        if (team == Team.A)
        {
#if UNITY_STANDALONE_WIN
            return Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.K) || Input.GetButtonDown("Fire1");
#else
            return joystick.GetFireButton();
#endif
        }
        else
        {
            return Input.GetKeyDown(KeyCode.Comma) || Input.GetKeyDown(KeyCode.KeypadPeriod) || Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.RightControl);
        }

    }

    public bool GetJumpButtonDown()
    {
        if (team == Team.A)
        {
#if UNITY_STANDALONE_WIN
            return Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump");
#else
            return joystick.GetJumpButton();
#endif
        }
        else
        {
            return Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return);
        }

    }

}