
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
        Provider.GameState.ToggleDebug();

    }

    [Button]
    public void RestartMatch()
    {
        Provider.GameState.RestartMatch();

    }

    [Button]
    public void AbortMatch()
    {
        Provider.GameState.AbortMatch();

    }

}