    using UnityEngine;
using UnityEngine.UI;

public class VolleyJoystick : MonoBehaviour
{
    [SerializeField] DynamicJoystick dynamicJoystick;
    [SerializeField] Button ButtonFire;
    [SerializeField] Button ButtonJump;

    int fireButtonQueried;
    int jumpButtonQueried;

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
        fireButtonQueried = 3;
        
    }

    public void HandleJumpButtonPress()
    {
        jumpButtonQueried = 3;
    }

    public bool GetFireButton()
    {
        fireButtonQueried--;
        return fireButtonQueried >= 0;

    }

    public bool GetJumpButton()
    {
        jumpButtonQueried--;
        return jumpButtonQueried >= 0;

    }

}