    using UnityEngine;
using UnityEngine.UI;

public class VolleyJoystick : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] bool simulateJoystick;
#endif
    [SerializeField] DynamicJoystick dynamicJoystick;
    [SerializeField] Button ButtonFire;
    [SerializeField] Button ButtonJump;

    int fireButtonQueried;
    int jumpButtonQueried;

    public float Horizontal
    {
        get
        {
#if UNITY_EDITOR
            return Input.GetAxisRaw("Horizontal");
#else
            return dynamicJoystick.Horizontal;
#endif
        }
    }

    public float Vertical
    {
        get
        {
#if UNITY_EDITOR
            return Input.GetAxisRaw("Vertical");
#else
            return dynamicJoystick.Vertical;
#endif
        }
    }

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