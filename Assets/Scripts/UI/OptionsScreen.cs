
using UnityEngine;

public class OptionsScreen : BaseView
{

    public void ToggleDebug()
    {
        Provider.Instance.GameState.ToggleDebug();
    }

    public void RestartMatch()
    {
        Provider.Instance.GameState.RestartMatch();
    }

    public void AbortMatch()
    {
        Provider.Instance.GameState.AbortMatch();
    }


}