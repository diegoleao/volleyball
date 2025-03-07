
using Sirenix.OdinInspector;
using UnityEngine;

public class OptionsScreen : BaseView
{

    [SerializeField] VirtualJoystick _virtualJoystick;
    public VirtualJoystick VirtualJoystick => _virtualJoystick;

    private MobileDebugBehaviour mobileDebugBehaviour;

    protected override void OnFirstExibition()
    {

    }

    void Awake()
    {
        mobileDebugBehaviour = FindAnyObjectByType<MobileDebugBehaviour>();

    }

    [Button]
    public void RegisterHiddenTouch()
    {
        mobileDebugBehaviour.ToggleConsoleAfter10Touches();

    }

    [Button]
    public void ToggleDebug()
    {
        Provider.Instance.GameState.ToggleDebug();

    }

    [Button]
    public void RestartMatch()
    {
        Provider.Instance.GameState.RestartMatch();

    }

    [Button]
    public void AbortMatch()
    {
        Provider.Instance.GameState.AbortMatch();

    }

}