    using UnityEngine;
using UnityEngine.UI;

public class VolleyJoystick : MonoBehaviour
{
    [SerializeField] DynamicJoystick dynamicJoystick;
    [SerializeField] Button ButtonFire;
    [SerializeField] Button ButtonJump;

    bool fireButtonPressed;
    bool jumpButtonPressed;

    public float Horizontal => dynamicJoystick.Horizontal;

    public float Vertical => dynamicJoystick.Vertical;

    void Start()
    {
#if UNITY_STANDALONE_WIN
        gameObject.SetActive(false);
#else
        gameObject.SetActive(true);
#endif

    }

    public void HandleFireButtonPress()
    {
        fireButtonPressed = true;
    }

    public void HandleJumpButtonPress()
    {
        jumpButtonPressed = true;
    }

    public bool GetFireButton()
    {
        if (fireButtonPressed)
        {
            fireButtonPressed = false;
            return true;
        }
        else
        {
            return false;
        }

    }

    public bool GetJumpButton()
    {
        if (jumpButtonPressed)
        {
            jumpButtonPressed = false;
            return true;
        }
        else
        {
            return false;
        }

    }

}